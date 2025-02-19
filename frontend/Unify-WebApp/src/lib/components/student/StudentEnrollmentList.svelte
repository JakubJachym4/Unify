<script lang="ts">
    import { onMount } from 'svelte';
    import { getStudentEnrollments, getGradeById, getEnrollmentById } from '$lib/api/Admin/Classes/ClassEnrollmentsRequests';
    import type { ClassEnrollment, Grade } from '$lib/types/universityClasses';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { user } from '$lib/stores/user';
    import { createEventDispatcher } from 'svelte';
	import EnrollmentGradeView from './EnrollmentGradeView.svelte';

    let enrollments: (ClassEnrollment & { 
        courseName?: string,
        className?: string
    })[] = [];
    let loading = true;
    let error = '';
    let selectedEnrollment: ClassEnrollment | null = null;
    let selectedGrade: boolean = false;
    let loadingGrade = false;
    let selectedAssignmentId: string;

    const dispatch = createEventDispatcher<{
        viewAssignment: { assignmentId: string };
    }>();

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            enrollments = await getStudentEnrollments($user!.id, token);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const loadGrade = async (enrollmentId: string) => {
        loadingGrade = true;
  
        selectedGrade = true;
        selectedEnrollment = enrollments.find(e => e.id === enrollmentId) || null; 
        loadingGrade = false;
   
    };

    const formatDateTime = (date: string): string => {
        return new Date(date).toLocaleString();
    };

    onMount(loadData);
</script>

<div class="container mt-4">
    {#if !selectedGrade}
    <h2>My Enrollments</h2>
    {/if}
        

        {#if error}
            <div class="alert alert-danger">{error}</div>
        {/if}

        {#if loading || loadingGrade}
            <div class="d-flex justify-content-center">
                <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        {:else if selectedGrade}
            <EnrollmentGradeView 
                enrollmentId={selectedEnrollment!.id} 
                onBack={() => selectedGrade = false}
                on:viewAssignment={(event) => {
                    selectedAssignmentId = event.detail.assignmentId;
                    selectedGrade = false;
                    dispatch('viewAssignment', { assignmentId: selectedAssignmentId });
                }}/>
        {:else}
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Course</th>
                            <th>Class</th>
                            <th>Enrolled On</th>
                            <th>Status</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {#each enrollments as enrollment}
                            <tr>
                                <td>{enrollment.courseName || 'N/A'}</td>
                                <td>{enrollment.className || 'N/A'}</td>
                                <td>{formatDateTime(enrollment.enrolledOn)}</td>
                                <td>
                                    {#if enrollment.grade?.dateAwarded}
                                        <span class="badge bg-success">
                                            <i class="bi bi-check-circle"></i> Graded
                                        </span>
                                    {:else}
                                        <span class="badge bg-info">
                                            <i class="bi bi-clock"></i> In Progress
                                        </span>
                                    {/if}
                                </td>
                                <td>
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => loadGrade(enrollment.id)}
                                        disabled={!enrollment.grade}
                                    >
                                        View Grade
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

    .card {
        box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
    }
</style>