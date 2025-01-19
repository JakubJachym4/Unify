<script lang="ts">
    import { onMount } from 'svelte';
    import { AddFaculty, UpdateFaculty, DeleteFaculty } from '$lib/api/Admin/Faculty/FacultyRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { facultiesStore } from '$lib/stores/university';
    import type { Faculty } from '$lib/types/university';

    let faculties: Faculty[] = [];
    let error = '';
    let loading = true;
    let newFaculty = { name: '' };
    let editingFaculty: Faculty | null = null;

    const loadFaculties = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await facultiesStore.load(token);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await AddFaculty(newFaculty.name, token);
            newFaculty = { name: '' };
            await loadFaculties();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async (faculty: Faculty | null) => {
        try {
            if(!faculty) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateFaculty({ id: faculty.id, name: faculty.name }, token);
            editingFaculty = null;
            await loadFaculties();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteFaculty(id, token);
            await loadFaculties();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    onMount(() => {
        const unsubscribe = facultiesStore.subscribe(value => {
            faculties = value;
        });
        loadFaculties();
        return unsubscribe;
    });
</script>

<div class="container mt-4">
    <h1 class="mb-4">Faculty Management</h1>

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
        <div class="card mb-4">
            <div class="card-body">
                <h5 class="card-title">Add New Faculty</h5>
                <div class="row g-3">
                    <div class="col-md-11">
                        <input 
                            class="form-control"
                            bind:value={newFaculty.name} 
                            placeholder="Faculty Name" 
                        />
                    </div>
                    <div class="col-md-1">
                        <button 
                            class="btn btn-primary w-100"
                            on:click={handleAdd}>
                            Add
                        </button>
                    </div>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each faculties as faculty}
                        <tr>
                            <td>{faculty.name}</td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingFaculty = faculty}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(faculty.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if editingFaculty}
            <div class="modal fade show" style="display: block;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Faculty</h5>
                            <button type="button" class="btn-close" on:click={() => editingFaculty = null}></button>
                        </div>
                        <div class="modal-body">
                            <div class="mb-3">
                                <input 
                                    class="form-control"
                                    bind:value={editingFaculty.name}
                                    placeholder="Faculty Name"
                                    required
                                />
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button 
                                class="btn btn-secondary"
                                on:click={() => editingFaculty = null}>
                                Cancel
                            </button>
                            <button 
                                class="btn btn-primary"
                                on:click={() => handleUpdate(editingFaculty)}>
                                Save
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-backdrop fade show"></div>
        {/if}
    {/if}
</div>