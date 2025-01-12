<script lang="ts">
    import { onMount } from 'svelte';
    import { getAllFieldsOfStudy, addFieldOfStudy, updateFieldOfStudy, deleteFieldOfStudy } from '$lib/api/Admin/FieldsOfStudy/FieldsOfStudyRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { Faculty, FieldOfStudy } from '$lib/types/university';
    import { fieldOfStudiesStore, facultiesStore } from '$lib/stores/university';

    let fieldsOfStudy: FieldOfStudy[] = [];
    let faculties: Faculty[] = [];
    let error = '';
    let loading = true;
    let searchTerm = '';
    let newField = { name: '', description: '', facultyId: '' };
    let editingField: FieldOfStudy | null = null;
    // Add form reference
    let addForm: HTMLFormElement;

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await Promise.all([
                fieldOfStudiesStore.load(token),
                facultiesStore.load(token)
            ]);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    // Modify handleAdd function
    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await addFieldOfStudy(newField, token);
            newField = { name: '', description: '', facultyId: '' };
            // Reset form validation state
            addForm.classList.remove('was-validated');
            addForm.reset();
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details || 'An error occurred';
        }
    };

    const handleUpdate = async (field: FieldOfStudy | null) => {
        try {
            if(!field) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await updateFieldOfStudy(field.id, { name: field.name, description: field.description }, token);
            editingField = null;
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await deleteFieldOfStudy(id, token);
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    $: filteredFields = fieldsOfStudy.filter(field => {
        const searchLower = searchTerm.toLowerCase();
        return field.name.toLowerCase().includes(searchLower) ||
               field.description.toLowerCase().includes(searchLower) ||
               faculties.find(f => f.id === field.facultyId)?.name.toLowerCase().includes(searchLower);
    });

    onMount(() => {
        const unsubscribeFields = fieldOfStudiesStore.subscribe(value => {
            fieldsOfStudy = value;
        });
        const unsubscribeFaculties = facultiesStore.subscribe(value => {
            faculties = value;
        });
        loadData();
        return () => {
            unsubscribeFields();
            unsubscribeFaculties();
        };
    });
</script>

<div class="container mt-4">
    <h1 class="mb-4">Fields of Study Management</h1>

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
                <h5 class="card-title">Add New Field of Study</h5>
                <form 
                    bind:this={addForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleAdd();
                        }
                        addForm.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="row g-3">
                        <div class="col-md-4">
                            <input 
                                class="form-control"
                                bind:value={newField.name} 
                                placeholder="Name" 
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a name (minimum 2 characters)
                            </div>
                        </div>
                        <div class="col-md-4">
                            <input 
                                class="form-control"
                                bind:value={newField.description} 
                                placeholder="Description" 
                                required
                                minlength="5"
                            />
                            <div class="invalid-feedback">
                                Please enter a description (minimum 5 characters)
                            </div>
                        </div>
                        <div class="col-md-3">
                            <select 
                                class="form-select"
                                bind:value={newField.facultyId}
                                required
                            >
                                <option value="">Select Faculty</option>
                                {#each faculties as faculty}
                                    <option value={faculty.id}>{faculty.name}</option>
                                {/each}
                            </select>
                            <div class="invalid-feedback">
                                Please select a faculty
                            </div>
                        </div>
                        <div class="col-md-1">
                            <button 
                                type="submit"
                                class="btn btn-primary w-100">
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
                    placeholder="Search fields..."
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
                        <th>Faculty</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredFields as field}
                        <tr>
                            <td>{field.name}</td>
                            <td>{field.description}</td>
                            <td>{faculties.find(f => f.id === field.facultyId)?.name}</td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingField = field}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(field.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if editingField}
            <div class="modal fade show" style="display: block;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <form 
                            on:submit|preventDefault={async (e) => {
                                    await handleUpdate(editingField);}}
                            class="needs-validation"
                            novalidate
                        >
                            <div class="modal-header">
                                <h5 class="modal-title">Edit Field of Study</h5>
                                <button type="button" class="btn-close" on:click={() => editingField = null}></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingField.name}
                                        placeholder="Name"
                                        required
                                    />
                                    <div class="invalid-feedback">
                                        Please enter a name
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingField.description}
                                        placeholder="Description"
                                        required
                                    />
                                    <div class="invalid-feedback">
                                        Please enter a description
                                    </div>
                                </div>
                               
                            <div class="modal-footer">
                                <button 
                                    type="button"
                                    class="btn btn-secondary"
                                    on:click={() => editingField = null}>
                                    Cancel
                                </button>
                                <button 
                                    type="submit"
                                    class="btn btn-primary">
                                    Save
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            </div>
            <div class="modal-backdrop fade show"></div>
        {/if}
    {/if}
</div>

<style>
</style>