<script lang="ts">
    import { onMount } from 'svelte';
    import type { ClassSession } from '$lib/types/resources';
    import { 
        CreateClassSession,
        UpdateClassSession,
        DeleteClassSession,
        CreateIntervalClassSessions,
        GetClassSessionByClassOffering 
    } from '$lib/api/Admin/Classes/ClassSessionRequests';
    import { GetAllLocations } from '$lib/api/Common/LocationRequests';
    import type { AcademicLocation } from '$lib/types/university';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { createEventDispatcher } from 'svelte';

    const dispatch = createEventDispatcher<{refresh: void}>();

    export let classOfferingId: string;
    export let onBack: () => void;

    let sessions: ClassSession[] = [];
    let locations: AcademicLocation[] = [];
    let error = '';
    let loading = true;
    let addingSession = false;
    let addingInterval = false;
    let editingSession: ClassSession | null = null;

    let newSession = {
        title: '',
        scheduledDate: '',
        duration: '90',
        lecturerId: '',
        locationId: '',
        classOfferingId: classOfferingId
    };

    let intervalSession = {
        title: '',
        startDate: '',
        endDate: '',
        weekInterval: 1,
        duration: '90',
        lecturerId: '',
        locationId: '',
        classOfferingId: classOfferingId
    };

    const loadSessions = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const data = await GetClassSessionByClassOffering(classOfferingId, token);
            sessions = data;
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const loadLocations = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            locations = await GetAllLocations(token);
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateClassSession(newSession, token);
            addingSession = false;
            dispatch('refresh');
            await loadSessions();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAddInterval = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateIntervalClassSessions(intervalSession, token);
            addingInterval = false;
            dispatch('refresh');
            await loadSessions();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async () => {
        if (!editingSession) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateClassSession({
                ...editingSession,
                duration: editingSession.duration.toString()
            }, token);
            editingSession = null;
            await loadSessions();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteClassSession({ id }, token);
            await loadSessions();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    onMount(async () => {
        await Promise.all([loadSessions(), loadLocations()]);
    });
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Class Sessions</h2>
        <div>
            <button class="btn btn-success me-2" on:click={() => addingInterval = true}>
                Create Interval
            </button>
            <button class="btn btn-primary me-2" on:click={() => addingSession = true}>
                Add Session
            </button>
            <button class="btn btn-secondary" on:click={onBack}>
                Back to Class
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
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Date</th>
                        <th>Time</th>
                        <th>Duration</th>
                        <th>Location</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each sessions as session}
                        {@const date = new Date(session.scheduledDate)}
                        <tr>
                            <td>{session.title}</td>
                            <td>{date.toLocaleDateString()}</td>
                            <td>{date.toLocaleTimeString()}</td>
                            <td>{session.duration} min</td>
                            <td>
                                {#if locations.find(l => l.id === session.locationId)}
                                    {@const location = locations.find(l => l.id === session.locationId)}
                                    {#if location.online}
                                        <a href={location.meetingUrl} target="_blank" rel="noopener noreferrer">
                                            Online Meeting
                                        </a>
                                    {:else}
                                        Building: {location.building}<br>
                                        Room: {location.doorNumber}
                                    {/if}
                                {/if}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingSession = {...session}}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(session.id)}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>

<!-- Add Session Modal -->
{#if addingSession}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={handleAdd}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Add New Session</h5>
                        <button type="button" class="btn-close" on:click={() => addingSession = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={newSession.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Date & Time</label>
                            <input 
                                type="datetime-local"
                                class="form-control"
                                bind:value={newSession.scheduledDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Duration (minutes)</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={newSession.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={newSession.locationId}
                                required
                            >
                                <option value="">Select Location</option>
                                {#each locations as location}
                                    <option value={location.id}>
                                        {location.online ? 
                                            `Online: ${location.meetingUrl}` : 
                                            `${location.building} - Room ${location.doorNumber}`}
                                    </option>
                                {/each}
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => addingSession = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Add Session
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Add Interval Modal -->
{#if addingInterval}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={handleAddInterval}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Create Session Interval</h5>
                        <button type="button" class="btn-close" on:click={() => addingInterval = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={intervalSession.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Start Date</label>
                            <input 
                                type="date"
                                class="form-control"
                                bind:value={intervalSession.startDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">End Date</label>
                            <input 
                                type="date"
                                class="form-control"
                                bind:value={intervalSession.endDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Week Interval</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={intervalSession.weekInterval}
                                required
                                min="1"
                                max="4"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Duration (minutes)</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={intervalSession.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={intervalSession.locationId}
                                required
                            >
                                <option value="">Select Location</option>
                                {#each locations as location}
                                    <option value={location.id}>
                                        {location.online ? 
                                            `Online: ${location.meetingUrl}` : 
                                            `${location.building} - Room ${location.doorNumber}`}
                                    </option>
                                {/each}
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => addingInterval = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Create Interval
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit Modal -->
{#if editingSession}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={handleUpdate}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Session</h5>
                        <button type="button" class="btn-close" on:click={() => editingSession = null}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={editingSession.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Date & Time</label>
                            <input 
                                type="datetime-local"
                                class="form-control"
                                bind:value={editingSession.scheduledDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Duration (minutes)</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={editingSession.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={editingSession.locationId}
                                required
                            >
                                <option value="">Select Location</option>
                                {#each locations as location}
                                    <option value={location.id}>
                                        {location.online ? 
                                            `Online: ${location.meetingUrl}` : 
                                            `${location.building} - Room ${location.doorNumber}`}
                                    </option>
                                {/each}
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingSession = null}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Update Session
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}