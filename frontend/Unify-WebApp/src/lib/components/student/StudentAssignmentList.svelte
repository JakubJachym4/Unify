<script lang="ts">
    import { onMount } from 'svelte';
    import { GetHomeworkAssignmentsByStudentId } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import { GetHomeworkSubmissionsByStudent } from '$lib/api/Admin/Assignments/HomeworkSubmissionsRequests';
    import type { HomeworkAssignment, HomeworkSubmission } from '$lib/types/resources';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { user } from '$lib/stores/user';
    import { createEventDispatcher } from 'svelte';

    let assignments: (HomeworkAssignment & { 
        courseName?: string, 
        className?: string,
        submissionStatus?: 'not_submitted' | 'submitted' | 'graded'
    })[] = [];
    let submissions: HomeworkSubmission[] = [];
    let loading = true;
    let error = '';
    let searchTerm = '';
    let statusFilter: 'all' | 'not_submitted' | 'submitted' | 'graded' = 'all';
    let sortKey: 'due_date' | 'title' | 'status' = 'due_date';

    const dispatch = createEventDispatcher<{
        viewAssignment: {  assignmentId: string };
    }>();

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            // Load assignments and submissions in parallel
            const [assignmentsData, submissionsData] = await Promise.all([
                GetHomeworkAssignmentsByStudentId($user!.id, token),
                GetHomeworkSubmissionsByStudent($user!.id, token)
            ]);

            submissions = submissionsData;

            assignments = assignmentsData.map(assignment => {
                const submission = submissions.find(s => s.assignmentId === assignment.id);
                return {
                    ...assignment,
                    submissionStatus: submission 
                        ? submission.mark 
                            ? 'graded' 
                            : 'submitted'
                        : 'not_submitted'
                };
            });

            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const formatDateTime = (date: string): string => {
        return new Date(date).toLocaleString();
    };

    $: filteredAssignments = assignments.filter(assignment => {
        const matchesSearch = 
            assignment.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
            assignment.description.toLowerCase().includes(searchTerm.toLowerCase());
        
        const matchesStatus = statusFilter === 'all' || assignment.submissionStatus === statusFilter;
        
        return matchesSearch && matchesStatus;
    }).sort((a, b) => {
        switch(sortKey) {
            case 'due_date':
                return new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime();
            case 'title':
                return a.title.localeCompare(b.title);
            case 'status':
                return (a.submissionStatus || '').localeCompare(b.submissionStatus || '');
            default:
                return 0;
        }
    });

    onMount(loadData);
</script>

<div class="container mt-4">
    <h2>My Assignments</h2>

    {#if error}
        <div class="alert alert-danger">{error}</div>
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
                <div class="row g-3">
                    <div class="col-md-5">
                        <input
                            type="search"
                            class="form-control"
                            placeholder="Search assignments..."
                            bind:value={searchTerm}
                        />
                    </div>
                    <div class="col-md-3">
                        <select class="form-select" bind:value={statusFilter}>
                            <option value="all">All Statuses</option>
                            <option value="not_submitted">Not Submitted</option>
                            <option value="submitted">Submitted</option>
                            <option value="graded">Graded</option>
                        </select>
                    </div>
                    <div class="col-md-4">
                        <select class="form-select" bind:value={sortKey}>
                            <option value="due_date">Sort by Due Date</option>
                            <option value="title">Sort by Title</option>
                            <option value="status">Sort by Status</option>
                        </select>
                    </div>
                </div>
            </div>
        </div>

        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Due Date</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredAssignments as assignment}
                        <tr>
                            <td>
                                <strong>{assignment.title}</strong>
                                {#if assignment.locked}
                                    <span class="badge bg-warning ms-2">
                                        <i class="bi bi-lock-fill"></i> Locked
                                    </span>
                                {/if}
                            </td>
                            <td>{assignment.description}</td>
                            <td>
                                <span class={new Date(assignment.dueDate) < new Date() ? 'text-danger' : ''}>
                                    {formatDateTime(assignment.dueDate)}
                                </span>
                            </td>
                            <td>
                                {#if assignment.submissionStatus === 'graded'}
                                    <span class="badge bg-success">
                                        <i class="bi bi-check-circle"></i> Graded
                                    </span>
                                {:else if assignment.submissionStatus === 'submitted'}
                                    <span class="badge bg-primary">
                                        <i class="bi bi-arrow-up-circle"></i> Submitted
                                    </span>
                                {:else}
                                    <span class="badge bg-warning text-dark">
                                        <i class="bi bi-exclamation-circle"></i> Not Submitted
                                    </span>
                                {/if}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary"
                                    on:click={() => dispatch('viewAssignment', {
                                        assignmentId: assignment.id
                                    })}
                                >
                                    View/Submit
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>

<style>
    .badge {
        font-size: 0.875rem;
    }

    .table td {
        vertical-align: middle;
    }

    .bi {
        margin-right: 0.25rem;
    }
</style>