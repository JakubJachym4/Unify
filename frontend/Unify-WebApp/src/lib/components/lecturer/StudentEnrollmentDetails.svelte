<script lang="ts">
    import { onMount } from 'svelte';
    import type { ClassEnrollment, Grade, Mark } from '$lib/types/universityClasses';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { AwardGrade } from '$lib/api/Admin/Classes/GradeRequests';
    
    export let enrollment: ClassEnrollment & { student?: UserResponse };
    export let onBack: () => void;

    let error = '';
    let loading = false;

    const formatDateTime = (date: string): string => {
        return new Date(date).toLocaleString();
    };

    const handleAwardToggle = async () => {
        if (!enrollment.grade) return;
        try {
            loading = true;
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await AwardGrade(enrollment.grade.id, !enrollment.grade.dateAwarded, token);
            // Refresh the enrollment data after toggle
            const updatedGrade = { 
                ...enrollment.grade,
                dateAwarded: !enrollment.grade.dateAwarded ? new Date().toISOString() : null
            };
            enrollment = { ...enrollment, grade: updatedGrade };
        } catch (err) {
            error = (err as ApiRequestError).details;
        } finally {
            loading = false;
        }
    };
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Student Enrollment Details</h2>
        <button class="btn btn-secondary" on:click={onBack}>Back</button>
    </div>

    {#if error}
        <div class="alert alert-danger alert-dismissible fade show" role="alert">
            {error}
            <button type="button" class="btn-close" on:click={() => error = ''}></button>
        </div>
    {/if}

    <div class="card mb-4">
        <div class="card-body">
            <h4>Student Information</h4>
            <p class="mb-1"><strong>Name:</strong> {enrollment.student?.firstName} {enrollment.student?.lastName}</p>
            <p class="mb-3"><strong>Email:</strong> {enrollment.student?.email}</p>
            <p><strong>Enrolled On:</strong> {formatDateTime(enrollment.enrolledOn)}</p>
        </div>
    </div>

    {#if enrollment.grade}
        <div class="card">
            <div class="card-body">
                <div class="d-flex justify-content-between align-items-start mb-4">
                    <h4 class="mb-0">Grade Details</h4>
                    <button 
                        class="btn {enrollment.grade.dateAwarded ? 'btn-warning' : 'btn-success'}"
                        on:click={handleAwardToggle}
                        disabled={loading}
                    >
                        {#if loading}
                            <span class="spinner-border spinner-border-sm me-1" role="status"></span>
                        {/if}
                        {enrollment.grade.dateAwarded ? 'Revoke Grade' : 'Award Grade'}
                    </button>
                </div>

                <div class="row mb-4">
                    <div class="col-md-6">
                        <h5>Overall Grade: {enrollment.grade.score.toFixed(1)}%</h5>
                        {#if enrollment.grade.dateAwarded}
                        <p class="text-muted">
                            Awarded on: {formatDateTime(enrollment.grade.dateAwarded)}
                        </p>
                        {/if}
                       
                    </div>
                </div>

                {#if enrollment.grade.description}
                    <div class="mb-4">
                        <h5>Comments</h5>
                        <p>{enrollment.grade.description}</p>
                    </div>
                {/if}

                {#if enrollment.grade.marks?.length}
                    <h5>Detailed Assessment</h5>
                    <div class="table-responsive">
                        <table class="table table-hover">
                            <thead>
                                <tr>
                                    <th>Title</th>
                                    <th class="text-end">Score</th>
                                    <th class="text-end">Max Score</th>
                                    <th class="text-end">Percentage</th>
                                </tr>
                            </thead>
                            <tbody>
                                {#each enrollment.grade.marks as mark}
                                    <tr>
                                        <td>{mark.title}</td>
                                        <td class="text-end">{mark.score}</td>
                                        <td class="text-end">{mark.maxScore}</td>
                                        <td class="text-end">
                                            {((mark.score / mark.maxScore) * 100).toFixed(1)}%
                                        </td>
                                    </tr>
                                {/each}
                                <tr class="table-info">
                                    <td><strong>Total</strong></td>
                                    <td class="text-end">
                                        <strong>{enrollment.grade.marks.reduce((sum, m) => sum + m.score, 0)}</strong>
                                    </td>
                                    <td class="text-end">
                                        <strong>{enrollment.grade.marks.reduce((sum, m) => sum + m.maxScore, 0)}</strong>
                                    </td>
                                    <td class="text-end">
                                        <strong>{enrollment.grade.score.toFixed(1)}%</strong>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                {/if}
            </div>
        </div>
    {:else}
        <div class="alert alert-info">
            <i class="bi bi-info-circle"></i> No grade has been assigned yet.
        </div>
    {/if}
</div>

<style>
    .table td, .table th {
        padding: 0.75rem;
    }

    .bi {
        margin-right: 0.5rem;
    }

    /* Add new styles */
    .btn:disabled {
        cursor: not-allowed;
        opacity: 0.65;
    }
</style>