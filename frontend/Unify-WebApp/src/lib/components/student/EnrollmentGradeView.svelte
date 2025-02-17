<script lang="ts">
    import { createEventDispatcher, onMount } from 'svelte';
    import { getEnrollmentById, getGradeById } from '$lib/api/Admin/Classes/ClassEnrollmentsRequests';
    import type { Grade, Mark } from '$lib/types/universityClasses';
    import type { ApiRequestError } from '$lib/api/apiError';
	import { GetHomeworkSubmission } from '$lib/api/Admin/Assignments/HomeworkSubmissionsRequests';
	import { get } from 'svelte/store';

    export let enrollmentId: string;
    export let onBack: () => void;
    const dispatch = createEventDispatcher<{
        viewAssignment: { assignmentId: string };
    }>();

    let grade: Grade | null = null;
    let loading = true;
    let error = '';

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const enrollment = await getEnrollmentById(enrollmentId, token);
            grade = enrollment.grade;
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const getAssignmentId = async (submissionId: string): Promise<string> => {
        const token = localStorage.getItem('token');
        if (!token) throw new Error('No token found');
        const submission = await GetHomeworkSubmission(submissionId, token);
        return submission.assignmentId;
    };

    const formatDateTime = (date: string): string => {
        return new Date(date).toLocaleString();
    };

    onMount(loadData);
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Grade Details</h2>
        <button class="btn btn-secondary" on:click={onBack}>Back</button>
    </div>

    {#if error}
        <div class="alert alert-danger">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else if grade}
        <div class="card">
            <div class="card-body">
                <div class="row mb-4">
                    <div class="col-md-6">
                        <h4 class="mb-3">Overall Grade: {grade.score}%</h4>
                        {#if grade.dateAwarded}
                            <p class="text-muted">
                            <i class="bi bi-calendar"></i> 
                            Awarded on: {formatDateTime(grade.dateAwarded)}
                        </p>
                        {/if}
                    </div>
                </div>

                {#if grade.description}
                    <div class="mb-4">
                        <h5>Description</h5>
                        <p>{grade.description}</p>
                    </div>
                {/if}

                {#if grade.marks?.length}
                    <h5>Detailed Assessment</h5>
                    <div class="table-responsive">
                        <table class="table table-hover">
                            <thead>
                                <tr>
                                    <th class="text-center">Title</th>
                                    <th class="text-center"></th>
                                    <th class="text-end">Score</th>
                                    <th class="text-end">Max Score</th>
                                    <th class="text-end">Percentage</th>
                                </tr>
                            </thead>
                            <tbody>
                                {#each grade.marks as mark}
                                    <tr>
                                        <td class="text-center">{mark.title}</td>
                                        {#if mark.submissionId}
                                            <td class="text-center">
                                                <button class="btn btn-secondary"
                                                on:click={async () => dispatch("viewAssignment", {
                                                    assignmentId: await getAssignmentId(mark.submissionId!)
                                                })}>View Submission</button>
                                            </td>
                                        {/if}
                                        <td class="text-end">{mark.score}</td>
                                        <td class="text-end">{mark.maxScore}</td>
                                        <td class="text-end">
                                            {((mark.score / mark.maxScore) * 100).toFixed(1)}%
                                        </td>
                                        
                                    </tr>
                                {/each}
                                <tr class="table-info">
                                    <td><strong>Total</strong></td>
                                    <td></td>
                                    <td class="text-end">
                                        <strong>
                                            {grade.marks.reduce((sum, mark) => sum + mark.score, 0)}
                                        </strong>
                                    </td>
                                    <td class="text-end">
                                        <strong>
                                            {grade.marks.reduce((sum, mark) => sum + mark.maxScore, 0)}
                                        </strong>
                                    </td>
                                    <td class="text-end">
                                        <strong>{grade.score}%</strong>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                {/if}
            </div>
        </div>
    {:else}
        <div class="alert alert-info">No grade information available.</div>
    {/if}
</div>

<style>
    .table td, .table th {
        padding: 0.75rem;
    }

    .bi {
        margin-right: 0.5rem;
    }
</style>