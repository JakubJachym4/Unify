<script lang="ts">
    import { onMount } from 'svelte';
    import { addSpecialization, updateSpecialization, deleteSpecialization } from '$lib/api/Admin/Specialization/SpecializationRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { Specialization, FieldOfStudy } from '$lib/types/university';
    import { specializationsStore, fieldOfStudiesStore } from '$lib/stores/university';

    let specializations: Specialization[] = [];
    let fieldsOfStudy: FieldOfStudy[] = [];
    let error = '';
    let loading = true;
    let searchTerm = '';
    let addForm: HTMLFormElement;

    let newSpecialization = {
        name: '',
        description: '',
        fieldOfStudyId: ''
    };

    let editingSpecialization: Specialization | null = null;

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await Promise.all([
                specializationsStore.load(token),
                fieldOfStudiesStore.load(token)
            ]);
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
            await addSpecialization(newSpecialization, token);
            newSpecialization = { name: '', description: '', fieldOfStudyId: '' };
            addForm.classList.remove('was-validated');
            addForm.reset();
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details || 'An error occurred';
        }
    };

    const handleUpdate = async (specialization: Specialization | null) => {
        try {
            if(!specialization) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await updateSpecialization(specialization.id, {
                name: specialization.name,
                description: specialization.description
            }, token);
            editingSpecialization = null;
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await deleteSpecialization(id, token);
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    $: filteredSpecializations = specializations.filter(spec => {
        const searchLower = searchTerm.toLowerCase();
        return spec.name.toLowerCase().includes(searchLower) ||
               spec.description.toLowerCase().includes(searchLower) ||
               fieldsOfStudy.find(f => f.id === spec.fieldOfStudyId)?.name.toLowerCase().includes(searchLower);
    });

    onMount(() => {
        const unsubscribeSpecs = specializationsStore.subscribe(value => {
            specializations = value;
        });
        const unsubscribeFields = fieldOfStudiesStore.subscribe(value => {
            fieldsOfStudy = value;
        });
        loadData();
        return () => {
            unsubscribeSpecs();
            unsubscribeFields();
        };
    });
</script>

<div class="container mt-4">
    <h1 class="mb-4">Specialization Management</h1>

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
                <h5 class="card-title">Add New Specialization</h5>
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
                        <div class="col-md-4">
                            <input 
                                class="form-control"
                                bind:value={newSpecialization.name} 
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
                                bind:value={newSpecialization.description} 
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
                                bind:value={newSpecialization.fieldOfStudyId}
                                required
                            >
                                <option value="">Select Field of Study</option>
                                {#each fieldsOfStudy as field}
                                    <option value={field.id}>{field.name}</option>
                                {/each}
                            </select>
                            <div class="invalid-feedback">
                                Please select a field of study
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
                    placeholder="Search specializations..."
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
                        <th>Field of Study</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredSpecializations as specialization}
                        <tr>
                            <td>{specialization.name}</td>
                            <td>{specialization.description}</td>
                            <td>{fieldsOfStudy.find(f => f.id === specialization.fieldOfStudyId)?.name}</td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingSpecialization = specialization}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(specialization.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if editingSpecialization}
            <div class="modal fade show" style="display: block;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <form 
                            on:submit|preventDefault={async (e) => {
                                if (e.target.checkValidity()) {
                                    await handleUpdate(editingSpecialization);
                                }
                                e.target.classList.add('was-validated');
                            }}
                            class="needs-validation"
                            novalidate
                        >
                            <div class="modal-header">
                                <h5 class="modal-title">Edit Specialization</h5>
                                <button type="button" class="btn-close" on:click={() => editingSpecialization = null}></button>
                            </div>
                            <div class="modal-body">
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingSpecialization.name}
                                        placeholder="Name"
                                        required
                                        minlength="2"
                                    />
                                    <div class="invalid-feedback">
                                        Please enter a name (minimum 2 characters)
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingSpecialization.description}
                                        placeholder="Description"
                                        required
                                        minlength="5"
                                    />
                                    <div class="invalid-feedback">
                                        Please enter a description (minimum 5 characters)
                                    </div>
                                </div>
                                <div class="mb-3">
                                    <select 
                                        class="form-select"
                                        bind:value={editingSpecialization.fieldOfStudyId}
                                        required
                                    >
                                        <option value="">Select Field of Study</option>
                                        {#each fieldsOfStudy as field}
                                            <option value={field.id}>{field.name}</option>
                                        {/each}
                                    </select>
                                    <div class="invalid-feedback">
                                        Please select a field of study
                                    </div>
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button 
                                    type="button"
                                    class="btn btn-secondary"
                                    on:click={() => editingSpecialization = null}>
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