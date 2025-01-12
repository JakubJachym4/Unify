<script lang="ts">
    import { onMount } from 'svelte';
    import { AddLocation, AddOnlineLocation, UpdateLocation, UpdateOnlineLocation, DeleteLocation } from '$lib/api/Common/LocationRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { Faculty, AcademicLocation } from '$lib/types/university';
    import { locationsStore, facultiesStore } from '$lib/stores/university';
    
    let locations: AcademicLocation[] = [];
    let faculties: Faculty[] = [];
    let error = '';
    let loading = true;
    let searchTerm = '';
    let addForm: HTMLFormElement;
    let isOnline = false;
    
    let newLocation = {
        building: '',
        street: '',
        floor: null as number | null,
        doorNumber: '',
        facultyId: '',
        meetingUrl: ''
    };
    
    let editingLocation: AcademicLocation | null = null;

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await Promise.all([
                locationsStore.load(token),
                facultiesStore.load(token)
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
            
            if (isOnline) {
                await AddOnlineLocation(newLocation.meetingUrl, token);
            } else {
                await AddLocation({
                    building: newLocation.building,
                    street: newLocation.street,
                    floor: Number(newLocation.floor),
                    doorNumber: newLocation.doorNumber,
                    facultyId: newLocation.facultyId || undefined
                }, token);
            }
            
            // Reset form
            newLocation = {
                building: '',
                street: '',
                floor: null,
                doorNumber: '',
                facultyId: '',
                meetingUrl: ''
            };
            addForm.classList.remove('was-validated');
            addForm.reset();
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details || 'An error occurred';
        }
    };

    const handleUpdate = async (location: AcademicLocation | null) => {
        try {
            if(!location) return;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            if (location.online) {
                await UpdateOnlineLocation({
                    id: location.id,
                    meetingUrl: location.meetingUrl!
                }, token);
            } else {
                await UpdateLocation({
                    id: location.id,
                    building: location.building!,
                    street: location.street!,
                    floor: location.floor!,
                    doorNumber: location.doorNumber!,
                    facultyId: location.facultyId
                }, token);
            }
            
            editingLocation = null;
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteLocation(id, token);
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    $: filteredLocations = locations.filter(location => {
        const searchLower = searchTerm.toLowerCase();
        return (
            (location.building?.toLowerCase().includes(searchLower) || false) ||
            (location.street?.toLowerCase().includes(searchLower) || false) ||
            (location.doorNumber?.toLowerCase().includes(searchLower) || false) ||
            (location.meetingUrl?.toLowerCase().includes(searchLower) || false)
        );
    });

    onMount(() => {
        const unsubscribeLocations = locationsStore.subscribe(value => {
            locations = value;
        });
        const unsubscribeFaculties = facultiesStore.subscribe(value => {
            faculties = value;
        });
        loadData();
        return () => {
            unsubscribeLocations();
            unsubscribeFaculties();
        };
    });
</script>

<div class="container mt-4">
    <h1 class="mb-4">Location Management</h1>

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
                <h5 class="card-title">Add New Location</h5>
                <div class="form-check mb-3">
                    <input 
                        class="form-check-input" 
                        type="checkbox" 
                        bind:checked={isOnline}
                        id="isOnlineCheck"
                    >
                    <label class="form-check-label" for="isOnlineCheck">
                        Online Location
                    </label>
                </div>
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
                    {#if isOnline}
                        <div class="row g-3">
                            <div class="col-md-11">
                                <input 
                                    type="url"
                                    class="form-control"
                                    bind:value={newLocation.meetingUrl} 
                                    placeholder="Meeting URL" 
                                    required
                                />
                                <div class="invalid-feedback">
                                    Please enter a valid URL
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
                    {:else}
                        <div class="row g-3">
                            <div class="col-md-3">
                                <input 
                                    class="form-control"
                                    bind:value={newLocation.building} 
                                    placeholder="Building" 
                                    required
                                    minlength="2"
                                />
                                <div class="invalid-feedback">
                                    Please enter building name
                                </div>
                            </div>
                            <div class="col-md-3">
                                <input 
                                    class="form-control"
                                    bind:value={newLocation.street} 
                                    placeholder="Street" 
                                    required
                                />
                                <div class="invalid-feedback">
                                    Please enter street name
                                </div>
                            </div>
                            <div class="col-md-2">
                                <input 
                                    type="number"
                                    class="form-control"
                                    bind:value={newLocation.floor} 
                                    placeholder="Floor"
                                    required
                                />
                                <div class="invalid-feedback">
                                    Please enter floor number
                                </div>
                            </div>
                            <div class="col-md-2">
                                <input 
                                    class="form-control"
                                    bind:value={newLocation.doorNumber} 
                                    placeholder="Door Number"
                                    required
                                />
                                <div class="invalid-feedback">
                                    Please enter door number
                                </div>
                            </div>
                            <div class="col-md-1">
                                <select 
                                    class="form-select"
                                    bind:value={newLocation.facultyId}
                                >
                                    <option value="">Faculty</option>
                                    {#each faculties as faculty}
                                        <option value={faculty.id}>{faculty.name}</option>
                                    {/each}
                                </select>
                            </div>
                            <div class="col-md-1">
                                <button 
                                    type="submit"
                                    class="btn btn-primary w-100">
                                    Add
                                </button>
                            </div>
                        </div>
                    {/if}
                </form>
            </div>
        </div>

        <div class="row mb-3">
            <div class="col">
                <input
                    type="search"
                    class="form-control"
                    placeholder="Search locations..."
                    bind:value={searchTerm}
                />
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Type</th>
                        <th>Details</th>
                        <th>Faculty</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredLocations as location}
                        <tr>
                            <td>{location.online ? 'Online' : 'Physical'}</td>
                            <td>
                                {#if location.online}
                                    <a href={location.meetingUrl} target="_blank" rel="noopener noreferrer">
                                        {location.meetingUrl}
                                    </a>
                                {:else}
                                    Building: {location.building}<br>
                                    Street: {location.street}<br>
                                    Floor: {location.floor}<br>
                                    Door: {location.doorNumber}
                                {/if}
                            </td>
                            <td>
                                {faculties.find(f => f.id === location.facultyId)?.name || '-'}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingLocation = location}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(location.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>

        {#if editingLocation}
            <div class="modal fade show" style="display: block;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Location</h5>
                            <button type="button" class="btn-close" on:click={() => editingLocation = null}></button>
                        </div>
                        <div class="modal-body">
                            {#if editingLocation.online}
                                <div class="mb-3">
                                    <input 
                                        type="url"
                                        class="form-control"
                                        bind:value={editingLocation.meetingUrl}
                                        placeholder="Meeting URL"
                                        required
                                    />
                                </div>
                            {:else}
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingLocation.building}
                                        placeholder="Building"
                                        required
                                    />
                                </div>
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingLocation.street}
                                        placeholder="Street"
                                        required
                                    />
                                </div>
                                <div class="mb-3">
                                    <input 
                                        type="number"
                                        class="form-control"
                                        bind:value={editingLocation.floor}
                                        placeholder="Floor"
                                        required
                                    />
                                </div>
                                <div class="mb-3">
                                    <input 
                                        class="form-control"
                                        bind:value={editingLocation.doorNumber}
                                        placeholder="Door Number"
                                        required
                                    />
                                </div>
                                <div class="mb-3">
                                    <select 
                                        class="form-select"
                                        bind:value={editingLocation.facultyId}
                                    >
                                        <option value="">Faculty</option>
                                        {#each faculties as faculty}
                                            <option value={faculty.id}>{faculty.name}</option>
                                        {/each}
                                    </select>
                                </div>
                            {/if}
                        </div>
                        <div class="modal-footer">
                            <button 
                                class="btn btn-secondary"
                                on:click={() => editingLocation = null}>
                                Cancel
                            </button>
                            <button 
                                class="btn btn-primary"
                                on:click={() => handleUpdate(editingLocation)}>
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