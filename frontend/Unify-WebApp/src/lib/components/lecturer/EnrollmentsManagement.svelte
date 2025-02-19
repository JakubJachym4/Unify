<script lang="ts">
	import { GetClassOffering } from '$lib/api/Admin/Classes/ClassOfferingsRequests';
    import { onMount } from 'svelte';
    import { getEnrollmentsByClassOffering, getStudentEnrollments } from '$lib/api/Admin/Classes/ClassEnrollmentsRequests';
    import type { ClassOffering, ClassEnrollment } from '$lib/types/universityClasses';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
	import StudentEnrollmentDetails from './StudentEnrollmentDetails.svelte';
	import { globalUsers } from '$lib/stores/globalUsers';
	import { get } from 'svelte/store';
    
    export let classOfferingId: string;
    export let onBack: () => void;

    let loading = true;
    let error = '';
    let classOffering: ClassOffering | null = null;
    let enrollments: (ClassEnrollment & { student?: UserResponse })[] = [];
    let selectedEnrollment: ClassEnrollment | null = null;
    let students: UserResponse[] = [];

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            students = get(globalUsers).filter(user => user.roles.includes('Student'));
            classOffering = await GetClassOffering(classOfferingId, token);
            const studentsEnrollments = await getEnrollmentsByClassOffering(classOfferingId, token);
            enrollments = studentsEnrollments.map(enrollment => {
                const student = students.find(user => user.id === enrollment.studentId);
                return { ...enrollment, student };
            });
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    onMount(loadData);
</script>

<div class="container mt-4">
    {#if !selectedEnrollment}
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Class Enrollments</h2>
            <button class="btn btn-secondary" on:click={onBack}>Back</button>
        </div>
    {/if}
   
    {#if error}
        <div class="alert alert-danger">{error}</div>
    {/if}

    {#if loading}
        <div class="d-flex justify-content-center">
            <div class="spinner-border" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>
    {:else if selectedEnrollment}
        <StudentEnrollmentDetails
            enrollment={selectedEnrollment}
            onBack={() => selectedEnrollment = null}
        />
    {:else}
        <div class="card">
            <div class="card-body">
                <div class="table-responsive">
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Student</th>
                                <th>Enrolled On</th>
                                <th>Grade Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {#each enrollments as enrollment}
                                <tr>
                                    <td>
                                        {enrollment.student?.firstName} {enrollment.student?.lastName}
                                        <br>
                                        <small class="text-muted">{enrollment.student?.email}</small>
                                    </td>
                                    <td>{new Date(enrollment.enrolledOn).toLocaleDateString()}</td>
                                    <td>
                                        {#if enrollment.grade?.dateAwarded}
                                            <span class="badge bg-success">
                                                <i class="bi bi-check-circle"></i> Graded
                                            </span>
                                        {:else}
                                            <span class="badge bg-warning text-dark">
                                                <i class="bi bi-clock"></i> Not Graded
                                            </span>
                                        {/if}
                                    </td>
                                    <td>
                                        <button 
                                            class="btn btn-sm btn-outline-primary"
                                            on:click={() => selectedEnrollment = enrollment}
                                        >
                                            View Details
                                        </button>
                                    </td>
                                </tr>
                            {/each}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    {/if}
</div>