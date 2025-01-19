<script lang="ts">
    import { onMount } from 'svelte';
    import type { StudentGroup } from '$lib/types/universityClasses';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { studentGroupsStore } from '$lib/stores/studentGroups';
    import { 
        getStudentGroupsBySpecializationId,
        createStudentGroup,
        createStudentGroupMultiple,
        updateStudentGroup,
        deleteStudentGroup,
        moveUserToGroup,

		autoAssignSpecializationStudentsToGroups

    } from '$lib/api/Common/StudentGroupRequests';
    import type { 
        CreateStudentGroupRequest,
        CreateStudentGroupForSpecializationRequest,
        UpdateStudentGroupRequest,
        MoveUserToGroupRequest 
    } from '$lib/api/Common/StudentGroupRequests';
	import type { Specialization } from '$lib/types/university';
	import type { UserResponse } from '$lib/api/User/UserRequests';

    export let specialization: Specialization;
    export let onBack: () => void;
    export let specializationUsers: UserResponse[] = [];


    let groups: StudentGroup[] = [];
    let error = '';
    let loading = true;
    let showMultipleGroupsForm = false;
    let addForm: HTMLFormElement;
    let editForm: HTMLFormElement;

    let newGroup: CreateStudentGroupRequest = {
        name: '',
        specializationId: specialization.id,
        studyYear: 1,
        semester: 1,
        term: 'Winter',
        maxGroupSize: 30
    };

    let multipleGroups: CreateStudentGroupForSpecializationRequest = {
        name: '',
        specializationId: specialization.id,
        studyYear: 1,
        semester: 1,
        term: 'Winter',
        combinedSize: 60,
        maxGroupSize: 30
    };

    let editingGroup: UpdateStudentGroupRequest | null = null;

    const loadGroups = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const fetchedGroups = await getStudentGroupsBySpecializationId(specialization.id, token);
            groups = fetchedGroups;
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const handleAddGroup = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await createStudentGroup(newGroup, token);
            newGroup = {
                name: '',
                specializationId: specialization.id,
                studyYear: 1,
                semester: 1,
                term: 'Winter',
                maxGroupSize: 30
            };
            addForm.classList.remove('was-validated');
            addForm.reset();
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAddMultipleGroups = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await createStudentGroupMultiple(multipleGroups, token);
            showMultipleGroupsForm = false;
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdateGroup = async () => {
        try {
            if (!editingGroup) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await updateStudentGroup(editingGroup, token);
            editingGroup = null;
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDeleteGroup = async (groupId: string) => {
        if (!confirm('Are you sure you want to delete this group?')) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await deleteStudentGroup(groupId, token);
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAutoAssignStudents = async () => {
        if (!confirm('Before auto assigning make sure none of the students is assigned to any group.')) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await autoAssignSpecializationStudentsToGroups(specialization.id, token);
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleMoveUser = async (userId: string, groupId: string | null) => {
        try {
            if(groupId === '')
                groupId = null;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await moveUserToGroup({ userId: userId, groupId: groupId }, token);
            await loadGroups();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    onMount(loadGroups);

</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Groups - {specialization.name}</h2>
        <div>
            <button class="btn btn-primary me-2" on:click={() => showMultipleGroupsForm = true}>
                Add Multiple Groups
            </button>
            <button class="btn btn-primary me-2" on:click={handleAutoAssignStudents}>
                Auto Assign Students
            </button>
            <button class="btn btn-secondary" on:click={onBack}>
                Back to Specializations
            </button>
        </div>
    </div>

    {#if error}
        <div class="alert alert-danger" role="alert">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else}
        <!-- Single group add form -->
        <div class="card mb-4">
            <div class="card-body">
                <h5 class="card-title">Add New Group</h5>
                <form 
                    bind:this={addForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleAddGroup();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="row g-3">
                        <div class="col-md-3">
                            <h6>Name</h6>
                            <input 
                                class="form-control"
                                bind:value={newGroup.name}
                                placeholder="Group Name"
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a group name (minimum 2 characters)
                            </div>
                        </div>
                        <div class="col-md-2">
                            <h6>Study Year</h6>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={newGroup.studyYear}
                                placeholder="Year"
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please enter a valid year
                            </div>
                        </div>
                        <div class="col-md-2">
                            <h6>Semester</h6>
                            <input 
                                type="number"
                                class="form-select"
                                bind:value={newGroup.semester}
                                placeholder="Semester"
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please select a semester
                            </div>
                        </div>
                        <div class="col-md-2">
                            <h6>Term</h6>
                            <select 
                                class="form-select"
                                bind:value={newGroup.term}
                                required
                            >
                                <option value="Winter">Winter</option>
                                <option value="Summer">Summer</option>
                            </select>
                            <div class="invalid-feedback">
                                Please select a term
                            </div>
                        </div>
                        <div class="col-md-2">
                            <h6>Max Group Size</h6>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={newGroup.maxGroupSize}
                                placeholder="Max Size"
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please enter a valid group size
                            </div>
                        </div>
                        <div class="col-md-1">
                            <br/>
                            <button type="submit" class="btn btn-primary w-100">Add</button>
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <!-- Groups list -->
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Year</th>
                        <th>Semester</th>
                        <th>Term</th>
                        <th>Size</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each groups as group}
                        <tr>
                            <td>{group.name}</td>
                            <td>{group.studyYear}</td>
                            <td>{group.semester}</td>
                            <td>{group.term}</td>
                            <td>{group.members.length}/{group.maxGroupSize}</td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingGroup = { ...group }}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDeleteGroup(group.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        <!-- Users assignment -->
        <div class="mt-4">
            <h6>Assign Users to Groups</h6>
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Current Group</th>
                        </tr>
                    </thead>
                    <tbody>
                        {#each specializationUsers as user}
                            <tr>
                                <td>{user.firstName} {user.lastName}</td>
                                <td>{user.email}</td>
                                <td>
                                    <select 
                                        class="form-select"
                                        on:change={(e) => handleMoveUser(user.id, e.target.value)}
                                    >
                                        <option value="" >No Group</option>
                                        {#each groups as group}
                                            <option 
                                                value={group.id}
                                                selected={group.members.includes(user.id)}
                                            >
                                                {group.name}
                                            </option>
                                        {/each}
                                    </select>
                                </td>
                            </tr>
                        {/each}
                    </tbody>
                </table>
            </div>
        </div>
    {/if}
</div>


<!-- Multiple groups modal -->
{#if showMultipleGroupsForm}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Multiple Groups</h5>
                    <button 
                        type="button" 
                        class="btn-close" 
                        on:click={() => showMultipleGroupsForm = false}
                    ></button>
                </div>
                <div class="modal-body">
                    <form class="needs-validation" novalidate>
                        <div class="mb-3">
                            <label class="form-label">Base Group Name</label>
                            <input 
                                class="form-control"
                                bind:value={multipleGroups.name}
                                required
                                minlength="2"
                            />
                            <div class="form-text">Groups will be named: name1, name2, etc.</div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Study Year</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={multipleGroups.studyYear}
                                required
                                min="1"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Semester</label>
                            <input 
                                type="number"
                                class="form-select"
                                bind:value={multipleGroups.semester}
                                placeholder="Semester"
                                required
                                min="1"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Term</label>
                            <select 
                                class="form-select"
                                bind:value={multipleGroups.term}
                                required
                            >
                                <option value="Winter">Winter</option>
                                <option value="Summer">Summer</option>
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Combined Size</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={multipleGroups.combinedSize}
                                required
                                min="1"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Max Group Size</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={multipleGroups.maxGroupSize}
                                required
                                min="1"
                            />
                            <div class="form-text">
                                This will create {Math.ceil(multipleGroups.combinedSize / multipleGroups.maxGroupSize)} groups
                            </div>
                        </div>
                    </form>
                </div>
                <div class="modal-footer">
                    <button 
                        class="btn btn-secondary"
                        on:click={() => showMultipleGroupsForm = false}>
                        Cancel
                    </button>
                    <button 
                        class="btn btn-primary"
                        on:click={handleAddMultipleGroups}>
                        Create Groups
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit group modal -->
{#if editingGroup}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    bind:this={editForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleUpdateGroup();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Group</h5>
                        <button 
                            type="button" 
                            class="btn-close" 
                            on:click={() => editingGroup = null}
                        ></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input 
                                class="form-control"
                                bind:value={editingGroup.name}
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a valid name (minimum 2 characters)
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Study Year</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={editingGroup.studyYear}
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please enter a valid year
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Semester</label>
                            <input 
                                type="number"
                                class="form-select"
                                bind:value={editingGroup.semester}
                                placeholder="Semester"
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please select a semester
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Term</label>
                            <select 
                                class="form-select"
                                bind:value={editingGroup.term}
                                required
                            >
                                <option value="Winter">Winter</option>
                                <option value="Summer">Summer</option>
                            </select>
                            <div class="invalid-feedback">
                                Please select a term
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Max Group Size</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={editingGroup.maxGroupSize}
                                required
                                min="1"
                            />
                            <div class="invalid-feedback">
                                Please enter a valid group size
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingGroup = null}>
                            Cancel
                        </button>
                        <button 
                            type="submit"
                            class="btn btn-primary">
                            Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

