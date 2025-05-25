<script lang="ts">
	import { onMount } from 'svelte';
    import type { ClassSession } from '$lib/types/resources';
    import { 
        CreateLecture, 
        UpdateLecture, 
        DeleteLecture,
        CreateIntervalLectures,
        GetLecturesByCourse 
    } from '$lib/api/Admin/Classes/LectureRequests';
    import { GetAllLocations } from '$lib/api/Common/LocationRequests';
    import type { AcademicLocation } from '$lib/types/university';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { createEventDispatcher } from 'svelte';
    import { globalUsers } from '$lib/stores/globalUsers';
    import type { UserResponse } from '$lib/api/User/UserRequests';
	import { get } from 'svelte/store';

    const dispatch = createEventDispatcher<{refresh: void}>();

    export let courseId: string;
    export let onBack: () => void;

    let lectures: ClassSession[] = [];
    let locations: AcademicLocation[] = [];
    let lecturers: UserResponse[] = [];
    let selectedLecturerId: string = '';
    let error = '';
    let loading = true;
    let addingLecture = false;
    let addingInterval = false;
    let editingLecture: ClassSession | null = null;
    

    let newLecture = {
        title: '',
        scheduledDate: '',
        duration: '90',
        lecturerId: '',
        locationId: '',
        courseId: courseId
    };

    let intervalLecture = {
        title: '',
        startDate: '',
        endDate: '',
        weekInterval: 1,
        duration: '90',
        lecturerId: '',
        locationId: '',
        courseId: courseId
    };

    const loadLectures = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const data = await GetLecturesByCourse(courseId, token);
            lectures = data;
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

    const loadLecturers = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const users = await get(globalUsers);
            lecturers = users.filter(u => u.roles.includes('Lecturer'));
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAdd = async () => {
        if (!newLecture.title || !selectedLecturerId) {
            error = 'Please fill all required fields';
            return;
        }
        
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateLecture({...newLecture, duration: newLecture.duration.toString(), lecturerId: selectedLecturerId}, token);
            addingLecture = false;
            dispatch('refresh');
            await loadLectures();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleAddInterval = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateIntervalLectures({...intervalLecture, lecturerId: selectedLecturerId, duration: intervalLecture.duration.toString()}, token);
            addingInterval = false;
            dispatch('refresh');
            await loadLectures();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async () => {
        if (!editingLecture) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateLecture({...editingLecture, duration: editingLecture.duration.toString()}, token);
            editingLecture = null;
            await loadLectures();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteLecture({ id }, token);
            await loadLectures();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

const formatTime = (date: Date): string => {
    const rawTime = date.getUTCHours().toString().padStart(2, '0') + ':' + date.getUTCMinutes().toString().padStart(2, '0');

    
    return rawTime;
};

    const getLecturerName = (lecturerId: string): string => {
        const lecturer = lecturers.find(l => l.id === lecturerId);
        return lecturer ? `${lecturer.firstName} ${lecturer.lastName}` : 'Unknown';
    };

    onMount(async () => {
        await Promise.all([loadLectures(), loadLocations(), loadLecturers()]);
    });

    $: sortedLectures = lectures.sort((a, b) => 
        new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime()
    );

    $: formattedDuration = (duration: string) => duration.split('.')[0];
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Lecture Management</h2>
        <div>
            <button class="btn btn-success me-2" on:click={() => addingInterval = true}>
                Create Interval
            </button>
            <button class="btn btn-primary me-2" on:click={() => addingLecture = true}>
                Add Lecture
            </button>
            <button class="btn btn-secondary" on:click={onBack}>
                Back to Course
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
                        <th>Lecturer</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each sortedLectures as lecture}
                        {@const date = new Date(lecture.scheduledDate)}
                        <tr>
                            <td>{lecture.title}</td>
                            <td>{date.toLocaleDateString()}</td>
                            <td>{formatTime(date)}</td>
                            <td>{formattedDuration(lecture.duration)} min</td>
                            <td>
                                {#if locations.find(l => l.id === lecture.locationId)}
                                    {@const location = locations.find(l => l.id === lecture.locationId)}
                                    {#if location.online}
                                        Online: {location.meetingUrl}
                                    {:else}
                                        Building: {location.building}, Room: {location.doorNumber}
                                    {/if}
                                {/if}
                            </td>
                            <td>
                                {getLecturerName(lecture.lecturerId)}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingLecture = {...lecture}}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(lecture.id)}>
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

<!-- Add Lecture Modal -->
{#if addingLecture}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={handleAdd}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Add New Lecture</h5>
                        <button type="button" class="btn-close" on:click={() => addingLecture = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={newLecture.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Date</label>
                            <input 
                                type="datetime-local"
                                class="form-control"
                                bind:value={newLecture.scheduledDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Duration (minutes)</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={newLecture.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={newLecture.locationId}
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
                        <div class="mb-3">
                            <label for="lecturer" class="form-label">Lecturer</label>
                            <select 
                                class="form-select" 
                                id="lecturer"
                                bind:value={selectedLecturerId}
                                required
                            >
                                <option value="">Select Lecturer</option>
                                {#each lecturers as lecturer}
                                    <option value={lecturer.id}>
                                        {lecturer.firstName} {lecturer.lastName}
                                    </option>
                                {/each}
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => addingLecture = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Add Lecture
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
                        <h5 class="modal-title">Create Lecture Interval</h5>
                        <button type="button" class="btn-close" on:click={() => addingInterval = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={intervalLecture.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Start Date</label>
                            <input 
                                type="datetime-local"
                                class="form-control"
                                bind:value={intervalLecture.startDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">End Date</label>
                            <input 
                                type="date"
                                class="form-control"
                                bind:value={intervalLecture.endDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Week Interval</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={intervalLecture.weekInterval}
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
                                bind:value={intervalLecture.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label for="lecturer" class="form-label">Lecturer</label>
                            <select 
                                class="form-select" 
                                id="lecturer"
                                bind:value={selectedLecturerId}
                                required
                            >
                                <option value="">Select Lecturer</option>
                                {#each lecturers as lecturer}
                                    <option value={lecturer.id}>
                                        {lecturer.firstName} {lecturer.lastName}
                                    </option>
                                {/each}
                            </select>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={intervalLecture.locationId}
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
{#if editingLecture}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    on:submit|preventDefault={handleUpdate}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Lecture</h5>
                        <button type="button" class="btn-close" on:click={() => editingLecture = null}></button>
                    </div>
                    <div class="modal-body">
                        <!-- Same form fields as Add Modal -->
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={editingLecture.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Date</label>
                            <input 
                                type="datetime-local"
                                class="form-control"
                                bind:value={editingLecture.scheduledDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Duration (minutes)</label>
                            <input 
                                type="number"
                                class="form-control"
                                bind:value={editingLecture.duration}
                                required
                                min="15"
                                step="15"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Location</label>
                            <select 
                                class="form-select"
                                bind:value={editingLecture.locationId}
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
                        <div class="mb-3">
                            <label for="editLecturer" class="form-label">Lecturer</label>
                            <select 
                                class="form-select" 
                                id="editLecturer"
                                bind:value={editingLecture.lecturerId}
                                required
                            >
                                {#each lecturers as lecturer}
                                    <option value={lecturer.id}>
                                        {lecturer.firstName} {lecturer.lastName}
                                    </option>
                                {/each}
                            </select>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingLecture = null}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Update Lecture
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}