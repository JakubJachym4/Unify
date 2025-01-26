<script lang="ts">
    import type { Course, ClassOffering, StudentGroup } from '$lib/types/universityClasses';
    import { CreateClassOffering, UpdateClassOffering, DeleteClassOffering, AssignLecturerToClassOffering, GetClassOffering } from '$lib/api/Admin/Classes/ClassOfferingsRequests';
    import { getStudentGroupsBySpecializationId } from '$lib/api/Common/StudentGroupRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
	import { onMount } from 'svelte';
	import { user } from '$lib/stores/user';
	import { get } from 'svelte/store';
	import { globalUsers } from '$lib/stores/globalUsers';
	import { getAllUsers, type UserResponse } from '$lib/api/User/UserRequests';
    import { createEventDispatcher } from 'svelte';
	import { GetCourseById } from '$lib/api/Admin/Classes/CourseRequests';
    const dispatch = createEventDispatcher<{refresh: void}>();

    // Add imports

    export let course: Course;
    export let onBack: () => void;

    let error = '';
    let editingOffering: ClassOffering | null = null;
    let addingOffering = false;
    let studentGroups: StudentGroup[] = [];
    let assigningLecturerToOffering: ClassOffering | null = null;
    let lecturerSearchTerm = '';
    let selectedLecturerId: string | null = null;
    let lecturers: UserResponse[] = []; // Assuming you have a Lecturer type and list
    let filteredLecturers = [];
    let userId = $user!.id;

    let newOffering: ClassOffering = {
        id: '',
        name: '',
        courseId: course.id,
        startDate: '',
        endDate: '',
        lecturerId: userId || '',
        studentGroupId: '',
        maxStudentsCount: 0,
    }

    const loadStudentGroups = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            studentGroups = await getStudentGroupsBySpecializationId(course.specialization.id, token);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    // Add lecturer loading
    const loadLecturers = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const users = await getAllUsers(token);
            lecturers = users.filter(user => user.roles.includes('Lecturer'));
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateClassOffering({
                ...newOffering,
                lecturerId: selectedLecturerId || ''
            }, token);
            course.classOfferings.push(newOffering);

            addingOffering = false;
            selectedLecturerId = null;
            dispatch('refresh');
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAssignLecturer = async () => {
        if (!assigningLecturerToOffering || !selectedLecturerId) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await AssignLecturerToClassOffering(
                assigningLecturerToOffering.id,
                selectedLecturerId,
                token
            );
        
            assigningLecturerToOffering = null;
            selectedLecturerId = null;
            lecturerSearchTerm = '';
            dispatch('refresh'); // Emit refresh event
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    }

    const handleUpdate = async () => {
        if (!editingOffering) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateClassOffering({...editingOffering}, token);
            let index = course.classOfferings.findIndex(offering => offering.id === editingOffering?.id);
            if (index !== -1) {
                let updatedOffering = await GetClassOffering(editingOffering.id, token);
                if (updatedOffering){
                    course.classOfferings[index] = updatedOffering;
                    
                }
            }
            editingOffering = null;
            dispatch('refresh'); // Emit refresh event
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteClassOffering({id}, token);
            let index = course.classOfferings.findIndex(offering => offering.id === id);

            if (index !== -1) {
                course.classOfferings.splice(index, 1);
            }
            dispatch('refresh'); // Emit refresh event
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    onMount(async () => {
        await Promise.all([
            loadStudentGroups(),
            loadLecturers()
        ]);
    });

$: filteredLecturers = lecturers.filter(lecturer => {
    if (!lecturerSearchTerm) return true;
    const searchLower = lecturerSearchTerm.toLowerCase();
    return lecturer.firstName.toLowerCase().includes(searchLower) ||
           lecturer.lastName.toLowerCase().includes(searchLower) ||
           lecturer.email.toLowerCase().includes(searchLower);
});

</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Classes - {course.name}</h2>
        <div>
            <button class="btn btn-primary me-2" on:click={() => addingOffering = true}>
                Add Offering
            </button>
            <button class="btn btn-secondary" on:click={onBack}>
                Back to Courses
            </button>
        </div>
    </div>

    {#if error}
        <div class="alert alert-danger" role="alert">{error}</div>
    {/if}

    <div class="table-responsive">
        <table class="table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Date</th>
                    <th>Student Group</th>
                    <th>Lecturer</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                {#each course.classOfferings as offering}
                {@const startDate = new Date(offering.startDate)}
                {@const endDate = new Date(offering.endDate)}
                    <tr>
                        <td>{offering.name}</td>
                        <td>{startDate.toLocaleDateString("en-GB")} - {endDate.toLocaleDateString("en-GB")}</td>

                        <td>
                            {#if offering.studentGroupId}
                                {#if studentGroups.find(g => g.id === offering.studentGroupId)}
                                    {@const group = studentGroups.find(g => g.id === offering.studentGroupId)}
                                    {group.name}
                                    <br/>
                                    <small class="text-muted">
                                        Students: {group.members.length}/{group.maxGroupSize}
                                    </small>
                                {:else}
                                    <span class="text-muted">Group not found</span>
                                {/if}
                            {:else}
                                <span class="text-muted">No group assigned</span>
                            {/if}
                        </td>
                        <td>
                            {#if offering.lecturerId}
                                {#if lecturers.find(l => l.id === offering.lecturerId)}
                                    {@const lecturer = lecturers.find(l => l.id === offering.lecturerId)}
                                    {lecturer.firstName} {lecturer.lastName}
                                    <br>
                                    <small class="text-muted">{lecturer.email}</small>
                                {:else}
                                    <span class="text-muted">Lecturer not found</span>
                                {/if}
                            {:else}
                                <button 
                                    class="btn btn-sm btn-outline-secondary"
                                    on:click={() => {
                                        assigningLecturerToOffering = offering;
                                        selectedLecturerId = null;
                                    }}
                                >
                                    Assign Lecturer
                                </button>
                            {/if}
                        </td>
                        <td>
                            <button 
                                class="btn btn-sm btn-outline-primary me-2"
                                on:click={() => editingOffering = {...offering}}>
                                Edit
                            </button>
                            <button 
                                class="btn btn-sm btn-outline-danger"
                                on:click={() => handleDelete(offering.id)}>
                                Delete
                            </button>
                        </td>
                    </tr>
                {/each}
            </tbody>
        </table>
    </div>
</div>


<!-- Add Modal -->
{#if addingOffering}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleAdd();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Add New Class Offering</h5>
                        <button type="button" class="btn-close" on:click={() => addingOffering = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input 
                                class="form-control"
                                bind:value={newOffering.name}
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a name (minimum 2 characters)
                            </div>
                        </div>
                        
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Start Date</label>
                                <input 
                                    type="date"
                                    class="datepicker"
                                    bind:value={newOffering.startDate}
                                    required
                                />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">End Date</label>
                                <input 
                                    type="date"
                                    class="datepicker"
                                    bind:value={newOffering.endDate}
                                    required
                                />
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Student Group</label>
                            <select 
                                class="form-select"
                                bind:value={newOffering.studentGroupId}
                                required
                            >
                                <option value="">Select a group</option>
                                {#each studentGroups as group}
                                    <option value={group.id}>
                                        {group.name} ({group.members.length}/{group.maxGroupSize})
                                    </option>  
                                {/each}
                            </select>
                            <div class="invalid-feedback">
                                Please select a student group
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Lecturer</label>
                            <div class="mb-2">
                                <input
                                    type="search"
                                    class="form-control"
                                    placeholder="Search lecturers..."
                                    bind:value={lecturerSearchTerm}
                                />
                            </div>
                            <select 
                                class="form-select"
                                bind:value={selectedLecturerId}
                                required
                            >
                                <option value="">Select a lecturer</option>
                                {#each filteredLecturers as lecturer}
                                    <option value={lecturer.id}>
                                        {lecturer.firstName} {lecturer.lastName} ({lecturer.email})
                                    </option>
                                {/each}
                            </select>
                            <div class="invalid-feedback">
                                Please select a lecturer
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => addingOffering = false}
                        >
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Add Offering
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit Modal -->
{#if editingOffering}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleUpdate();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Class Offering</h5>
                        <button type="button" class="btn-close" on:click={() => editingOffering = null}></button>
                    </div>
                    <div class="modal-body">
                        <!-- Same form fields as Add Modal -->
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input 
                                class="form-control"
                                bind:value={editingOffering.name}
                                required
                                minlength="2"
                            />
                        </div>
                        
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Start Date</label>
                                <input 
                                    type="date"
                                    class="datepicker"
                                    bind:value={editingOffering.startDate}
                                    required
                                />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">End Date</label>
                                <input 
                                    type="date"
                                    class="datepicker"
                                    bind:value={editingOffering.endDate}
                                    required
                                />
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Lecturer</label>
                            <div class="mb-2">
                                <input
                                    type="search"
                                    class="form-control"
                                    placeholder="Search lecturers..."
                                    bind:value={lecturerSearchTerm}
                                />
                            </div>
                            <select 
                                class="form-select"
                                bind:value={selectedLecturerId}
                                required
                            >
                                <option value="">Select a lecturer</option>
                                {#each filteredLecturers as lecturer}
                                    <option value={lecturer.id}>
                                        {lecturer.firstName} {lecturer.lastName} ({lecturer.email})
                                    </option>
                                {/each}
                            </select>
                            <div class="invalid-feedback">
                                Please select a lecturer
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingOffering = null}
                        >
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Assign Lecturer Modal -->
{#if assigningLecturerToOffering}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Assign Lecturer to {assigningLecturerToOffering.name}</h5>
                    <button type="button" class="btn-close" on:click={() => assigningLecturerToOffering = null}></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <input
                            type="search"
                            class="form-control mb-3"
                            placeholder="Search lecturers..."
                            bind:value={lecturerSearchTerm}
                        />
                        <select 
                            class="form-select"
                            bind:value={selectedLecturerId}
                            size="5"
                            required
                        >
                            <option value="">Choose a lecturer</option>
                            {#each filteredLecturers as lecturer}
                                <option value={lecturer.id}>
                                    {lecturer.firstName} {lecturer.lastName} ({lecturer.email})
                                </option>
                            {/each}
                        </select>
                    </div>
                </div>
                <div class="modal-footer">
                    <button 
                        class="btn btn-secondary"
                        on:click={() => {
                            assigningLecturerToOffering = null;
                            lecturerSearchTerm = '';
                        }}
                    >
                        Cancel
                    </button>
                    <button 
                        class="btn btn-primary"
                        disabled={!selectedLecturerId}
                        on:click={handleAssignLecturer}
                    >
                        Assign
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}