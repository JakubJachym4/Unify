<script lang="ts">
    import { onMount } from 'svelte';
    import { getStudentInformation } from '$lib/api/Common/UniversityInfoRequest';
    import type { StudentInformationResponse } from '$lib/api/Common/UniversityInfoRequest';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { user } from '$lib/stores/user';

    let studentInfo: StudentInformationResponse | null = null;
    let error = '';
    let loading = true;

    const loadStudentInfo = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            studentInfo = await getStudentInformation(token);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    onMount(loadStudentInfo);
</script>

<div class="container mt-4">
    <div class="card">
        <div class="card-header">
            <h4 class="mb-0">Student Information</h4>
        </div>
        <div class="card-body">
            {#if error}
                <!-- Show basic user information when not enrolled -->
                <div class="row">
                    <div class="col-md-6">
                        <div class="alert alert-info mb-4">
                            <i class="bi bi-info-circle me-2"></i>
                            You are not currently enrolled in any classes.
                        </div>
                        <h5 class="mb-4">Basic Information</h5>
                        <dl class="row">
                            <dt class="col-sm-4">Name</dt>
                            <dd class="col-sm-8">{$user?.firstName} {$user?.lastName}</dd>
                            
                            <dt class="col-sm-4">Email</dt>
                            <dd class="col-sm-8">{$user?.email}</dd>
                        </dl>
                    </div>
                </div>
            {:else if loading}
                <div class="d-flex justify-content-center">
                    <div class="spinner-border" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>
            {:else}
                <div class="row">
                    <div class="col-md-6">
                        <h5 class="mb-4">Personal Information</h5>
                        <dl class="row">
                            <dt class="col-sm-4">Name</dt>
                            <dd class="col-sm-8">{studentInfo?.firstName} {studentInfo?.lastName}</dd>
                            
                            <dt class="col-sm-4">Email</dt>
                            <dd class="col-sm-8">{studentInfo?.email}</dd>
                        </dl>
                    </div>
                    <div class="col-md-6">
                        <h5 class="mb-4">Academic Information</h5>
                        <dl class="row">
                            <dt class="col-sm-4">Faculty</dt>
                            <dd class="col-sm-8">{studentInfo?.faculty}</dd>
                            
                            <dt class="col-sm-4">Field of Study</dt>
                            <dd class="col-sm-8">{studentInfo?.fieldOfStudy}</dd>
                            
                            <dt class="col-sm-4">Specialization</dt>
                            <dd class="col-sm-8">{studentInfo?.specialization}</dd>
                            
                            <dt class="col-sm-4">Student Group</dt>
                            <dd class="col-sm-8">{studentInfo?.studentGroup}</dd>
                            
                            <dt class="col-sm-4">Study Year</dt>
                            <dd class="col-sm-8">{studentInfo?.studyYear}</dd>
                            
                            <dt class="col-sm-4">Semester</dt>
                            <dd class="col-sm-8">{studentInfo?.semester}</dd>
                            
                            <dt class="col-sm-4">Term</dt>
                            <dd class="col-sm-8">{studentInfo?.term}</dd>
                        </dl>
                    </div>
                </div>
            {/if}
        </div>
    </div>
</div>

<style>
    dt {
        font-weight: 500;
        color: var(--bs-secondary);
    }

    dd {
        margin-bottom: 0.5rem;
    }

    .alert-info {
        background-color: rgba(var(--bs-info-rgb), 0.1);
        border: 1px solid rgba(var(--bs-info-rgb), 0.2);
    }

    h5 {
        color: var(--bs-primary);
        font-weight: 500;
    }

    .card {
        box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
    }
</style>