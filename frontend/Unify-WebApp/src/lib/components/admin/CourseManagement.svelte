<script lang="ts">
	import { fieldOfStudiesStore } from '../../stores/university';
    import { onMount } from 'svelte';
    import { courseStore } from '$lib/stores/course';
    import { specializationsStore } from '$lib/stores/university';
    import { CreateCourse, UpdateCourse, DeleteCourse, GetCourseBySpecialization, AssignLecturer } from '$lib/api/Admin/Classes/CourseRequests';
    import { getAllUsers, type UserResponse } from '$lib/api/User/UserRequests';
    import type { Course } from '$lib/types/universityClasses';
    import type { CreateCourseRequest } from '$lib/api/Admin/Classes/CourseRequests';
    import type { FieldOfStudy, Specialization } from '$lib/types/university';
    import type { ApiRequestError } from '$lib/api/apiError';

    let specializations: Specialization[] = [];
    let fieldsOfStudy: FieldOfStudy[] = [];
    let courses: Course[] = [];
    let selectedSpecialization: Specialization | null = null;
    let error = '';
    let loading = true;
    let searchTerm = '';
    let addForm: HTMLFormElement;
    let editingCourse: Course | null = null;
    let deletingCourse: Course | null = null;
    let assigningLecturerToCourse: Course | null = null;
    let lecturers: UserResponse[] = [];
    let selectedLecturerId = '';
    let lecturerSearchTerm = '';

    let newCourse = {
        name: '',
        description: '',
        specializationId: ''
    };

    $: filteredSpecializations = 
    specializations.filter(spec => {
        const fieldOfStudy = fieldsOfStudy.find(fos => fos.id === spec.fieldOfStudyId);
        return fieldOfStudy && fieldOfStudy.name.toLowerCase().includes(searchTerm.toLowerCase());
    });

    $: filteredLecturers = lecturers.filter(lecturer => {
        const searchLower = lecturerSearchTerm.toLowerCase();
        return lecturer.firstName.toLowerCase().includes(searchLower) ||
               lecturer.lastName.toLowerCase().includes(searchLower) ||
               lecturer.email.toLowerCase().includes(searchLower);
    });

    const loadSpecializations = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await specializationsStore.load(token);
            await fieldOfStudiesStore.load(token);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const loadCourses = async (specializationId: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const courses = await GetCourseBySpecialization(specializationId, token);
            courseStore.set(courses.map(course => ({
                ...course,
                specialization: selectedSpecialization!,
                lecturer: lecturers.find(l => l.id == course.lecturerId) ?? null,
                classOfferings: []
            })));
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleSpecializationSelect = async (specialization: Specialization) => {
        selectedSpecialization = specialization;
        searchTerm = '';
        await loadCourses(specialization.id);
    };

    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateCourse({
                ...newCourse,
                specializationId: selectedSpecialization!.id
            }, token);
            newCourse = { name: '', description: '', specializationId: '' };
            addForm.classList.remove('was-validated');
            addForm.reset();
            await loadCourses(selectedSpecialization!.id);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async () => {
        if (!editingCourse) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateCourse({
                id: editingCourse.id,
                name: editingCourse.name,
                description: editingCourse.description
            }, token);
            editingCourse = null;
            await loadCourses(selectedSpecialization!.id);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async () => {
        if (!deletingCourse) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteCourse({ id: deletingCourse.id }, token);
            deletingCourse = null;
            await loadCourses(selectedSpecialization!.id);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

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

    const handleAssignLecturer = async () => {
        try {
            if (!assigningLecturerToCourse || !selectedLecturerId) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await AssignLecturer(assigningLecturerToCourse.id, selectedLecturerId, token);
            assigningLecturerToCourse = null;
            selectedLecturerId = '';
            await loadCourses(selectedSpecialization!.id);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    $: filteredCourses = courses.filter(course => {
        const searchLower = searchTerm.toLowerCase();
        return course.name.toLowerCase().includes(searchLower) ||
               course.description.toLowerCase().includes(searchLower);
    });

    onMount(() => {
        loadLecturers();
        const unsubscribeSpecs = specializationsStore.subscribe(value => {
            specializations = value;
        });
        const unsubscribeCourses = courseStore.subscribe(value => {
            courses = value;
        });
        const unsubscribeFields = fieldOfStudiesStore.subscribe(value => {
            fieldsOfStudy = value;
        });
        loadSpecializations();
        return () => {
            unsubscribeSpecs();
            unsubscribeCourses();
            unsubscribeFields();
        };
    });
</script>

<div class="container mt-4">
    <h1 class="mb-4">Course Management</h1>

    {#if error}
        <div class="alert alert-danger" role="alert">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else if !selectedSpecialization}
        <div class="card">
            <div class="card-body">
                <h5 class="card-title">Select Specialization</h5>
                <input
                    type="search"
                    class="form-control mb-3"
                    placeholder="Search specializations..."
                    bind:value={searchTerm}
                />
                <div class="list-group">
                    {#each filteredSpecializations as specialization}
                        <button 
                            class="list-group-item list-group-item-action"
                            on:click={() => handleSpecializationSelect(specialization)}
                        >
                            {fieldsOfStudy.find(fos => fos.id === specialization.fieldOfStudyId)?.name} : {specialization.name}
                        </button>
                    {/each}
                </div>
            </div>
        </div>
    {:else}
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>{selectedSpecialization.name} - Courses</h2>
            <button 
                class="btn btn-secondary"
                on:click={() => selectedSpecialization = null}
            >
                Back to Specializations
            </button>
        </div>

        <div class="card mb-4">
            <div class="card-body">
                <h5 class="card-title">Add New Course</h5>
                <form 
                    bind:this={addForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleAdd();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="row g-3">
                        <div class="col-md-5">
                            <input 
                                class="form-control"
                                bind:value={newCourse.name}
                                placeholder="Course Name"
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a course name (minimum 2 characters)
                            </div>
                        </div>
                        <div class="col-md-6">
                            <input 
                                class="form-control"
                                bind:value={newCourse.description}
                                placeholder="Description"
                                required
                                minlength="5"
                            />
                            <div class="invalid-feedback">
                                Please enter a description (minimum 5 characters)
                            </div>
                        </div>
                        <div class="col-md-1">
                            <button 
                                type="submit"
                                class="btn btn-primary w-100"
                            >
                                Add
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col">
                <input
                    type="search"
                    class="form-control"
                    placeholder="Search courses..."
                    bind:value={searchTerm}
                />
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Description</th>
                        <th>Lecturer</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredCourses as course}
                        <tr>
                            <td>{course.name}</td>
                            <td>{course.description}</td>
                            <td>
                                {#if course.lecturer}
                                    {course.lecturer.firstName} {course.lecturer.lastName}
                                    <br>
                                    <small class="text-muted">{course.lecturer.email}</small>
                                {:else}
                                    <span class="text-muted">No lecturer assigned</span>
                                {/if}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingCourse = {...course}}
                                >
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => deletingCourse = course}
                                >
                                    Delete
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-info me-2"
                                    on:click={() => {
                                        assigningLecturerToCourse = course;
                                        selectedLecturerId = '';
                                    }}
                                >
                                    Assign Lecturer
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>

<!-- Edit Modal -->
{#if editingCourse}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Course</h5>
                    <button 
                        type="button" 
                        class="btn-close" 
                        on:click={() => editingCourse = null}
                    ></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <input 
                            class="form-control"
                            bind:value={editingCourse.name}
                            placeholder="Course Name"
                            required
                        />
                    </div>
                    <div class="mb-3">
                        <input 
                            class="form-control"
                            bind:value={editingCourse.description}
                            placeholder="Description"
                            required
                        />
                    </div>
                </div>
                <div class="modal-footer">
                    <button 
                        class="btn btn-secondary"
                        on:click={() => editingCourse = null}
                    >
                        Cancel
                    </button>
                    <button 
                        class="btn btn-primary"
                        on:click={handleUpdate}
                    >
                        Save Changes
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Delete Confirmation Modal -->
{#if deletingCourse}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Delete</h5>
                    <button 
                        type="button" 
                        class="btn-close" 
                        on:click={() => deletingCourse = null}
                    ></button>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to delete course "{deletingCourse.name}"?</p>
                </div>
                <div class="modal-footer">
                    <button 
                        class="btn btn-secondary"
                        on:click={() => deletingCourse = null}
                    >
                        Cancel
                    </button>
                    <button 
                        class="btn btn-danger"
                        on:click={handleDelete}
                    >
                        Delete
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Assign Lecturer Modal -->
{#if assigningLecturerToCourse}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Assign Lecturer to {assigningLecturerToCourse.name}</h5>
                    <button 
                        type="button" 
                        class="btn-close" 
                        on:click={() => assigningLecturerToCourse = null}
                    ></button>
                </div>
                <div class="modal-body">
                    <div class="mb-3">
                        <label class="form-label">Search Lecturers</label>
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
                            assigningLecturerToCourse = null;
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