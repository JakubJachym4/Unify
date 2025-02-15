<script lang="ts">
    import { onMount } from 'svelte';
    import { GetClassOfferingsByLecturer } from '$lib/api/Admin/Classes/ClassOfferingsRequests';
    import { GetClassSessionByClassOffering } from '$lib/api/Admin/Classes/ClassSessionRequests';
    import { GetHomeworkAssignmentsByClassOfferingId } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { ClassOffering, Course } from '$lib/types/universityClasses';
    import type { ClassSession } from '$lib/types/resources';
    import type { HomeworkAssignment } from '$lib/types/resources';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { user } from '$lib/stores/user';
    import { GetCourseById } from '$lib/api/Admin/Classes/CourseRequests';
    import { createEventDispatcher } from 'svelte';

    let loading = true;
    let error = '';
    let upcomingSessions: (ClassSession & { courseName?: string, className?: string })[] = [];
    let upcomingAssignments: (HomeworkAssignment & { courseName?: string, className?: string })[] = [];

    const DAYS_TO_SHOW = 7; // Show items for next 7 days

    const dispatch = createEventDispatcher<{
        openSession: { classOfferingId: string };
        openAssignment: { classOfferingId: string };
    }>();

    const loadDashboardData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            // Get all class offerings for the lecturer
            const offerings = await GetClassOfferingsByLecturer($user!.id, token);

            // Load sessions and assignments for each offering
            const sessionsPromises = offerings.map(async (offering) => {
                const sessions = await GetClassSessionByClassOffering(offering.id, token);
                const course = await GetCourseById(offering.courseId, token);
                return sessions.map(session => ({
                    ...session,
                    courseName: course.name,
                    className: offering.name
                }));
            });

            const assignmentsPromises = offerings.map(async (offering) => {
                const assignments = await GetHomeworkAssignmentsByClassOfferingId(offering.id, token);
                const course = await GetCourseById(offering.courseId, token);
                return assignments.map(assignment => ({
                    ...assignment,
                    courseName: course.name,
                    className: offering.name
                }));
            });

            const allSessions = (await Promise.all(sessionsPromises)).flat();
            const allAssignments = (await Promise.all(assignmentsPromises)).flat();

            const now = new Date();
            const futureDate = new Date();
            futureDate.setDate(now.getDate() + DAYS_TO_SHOW);

            // Filter and sort upcoming sessions
            upcomingSessions = allSessions
                .filter(session => {
                    const sessionDate = new Date(session.scheduledDate);
                    return sessionDate >= now && sessionDate <= futureDate;
                })
                .sort((a, b) => new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime());

            // Filter and sort upcoming assignment deadlines
            upcomingAssignments = allAssignments
                .filter(assignment => {
                    const dueDate = new Date(assignment.dueDate);
                    return dueDate >= now && dueDate <= futureDate;
                })
                .sort((a, b) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime());

            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const formatDateTime = (date: Date): string => {
        return new Date(date).toLocaleString();
    };

    const handleSessionClick = (classOfferingId: string) => {
        dispatch('openSession', { classOfferingId });
    };

    const handleAssignmentClick = (classOfferingId: string) => {
        dispatch('openAssignment', { classOfferingId });
    };

    onMount(loadDashboardData);
</script>

<div class="container mt-4">
    <h2 class="mb-4">Lecturer Dashboard</h2>

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
        <div class="row">
            <!-- Upcoming Sessions -->
            <div class="col-md-6 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Upcoming Sessions</h5>
                    </div>
                    <div class="card-body">
                        {#if upcomingSessions.length === 0}
                            <p class="text-muted">No upcoming sessions in the next {DAYS_TO_SHOW} days</p>
                        {:else}
                            <div class="list-group">
                                {#each upcomingSessions as session}
                                    <div 
                                        class="list-group-item list-group-item-action dashboard-item" 
                                        on:click={() => handleSessionClick(session.classOfferingId)}
                                        role="button"
                                    >
                                        <div class="d-flex w-100 justify-content-between align-items-start mb-2">
                                            <h5 class="mb-1">{session.title}</h5>
                                            <span class="badge bg-primary rounded-pill">
                                                {formatDateTime(session.scheduledDate)}
                                            </span>
                                        </div>
                                        <h6 class="mb-2">{session.courseName}</h6>
                                        <p class="mb-1">Class: {session.className}</p>
                                        <div class="d-flex justify-content-between align-items-center">
                                            <small class="text-muted">Duration: {session.duration} minutes</small>
                                            <button class="btn btn-sm btn-outline-primary">
                                                View Details
                                            </button>
                                        </div>
                                    </div>
                                {/each}
                            </div>
                        {/if}
                    </div>
                </div>
            </div>

            <!-- Upcoming Assignment Deadlines -->
            <div class="col-md-6 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Upcoming Assignment Deadlines</h5>
                    </div>
                    <div class="card-body">
                        {#if upcomingAssignments.length === 0}
                            <p class="text-muted">No upcoming deadlines in the next {DAYS_TO_SHOW} days</p>
                        {:else}
                            <div class="list-group">
                                {#each upcomingAssignments as assignment}
                                    <div 
                                        class="list-group-item list-group-item-action dashboard-item"
                                        on:click={() => handleAssignmentClick(assignment.classOfferingId)}
                                        role="button"
                                    >
                                        <div class="d-flex w-100 justify-content-between align-items-start mb-2">
                                            <h5 class="mb-1">{assignment.title}</h5>
                                            <span class="badge bg-danger rounded-pill">
                                                Due: {formatDateTime(assignment.dueDate)}
                                            </span>
                                        </div>
                                        <h6 class="mb-2">{assignment.courseName}</h6>
                                        <p class="mb-1">Class: {assignment.className}</p>
                                        <div class="d-flex justify-content-between align-items-center">
                                            <small class="text-muted">
                                                {#if assignment.submissions?.length}
                                                    Submissions: {assignment.submissions.length}
                                                {:else}
                                                    No submissions yet
                                                {/if}
                                            </small>
                                            <button class="btn btn-sm btn-outline-primary">
                                                Manage Assignment
                                            </button>
                                        </div>
                                    </div>
                                {/each}
                            </div>
                        {/if}
                    </div>
                </div>
            </div>
        </div>
    {/if}
</div>

<style>
    .dashboard-item {
        padding: 1.25rem;
        margin-bottom: 0.5rem;
        border-radius: 0.5rem;
        transition: all 0.2s ease-in-out;
    }

    .dashboard-item:hover {
        background-color: #f8f9fa;
        transform: translateY(-2px);
        box-shadow: 0 0.25rem 0.5rem rgba(0, 0, 0, 0.1);
    }

    .card {
        box-shadow: 0 0.125rem 0.25rem rgba(0, 0, 0, 0.075);
        border-radius: 0.5rem;
    }

    .card-header {
        background-color: #f8f9fa;
        border-bottom: 1px solid rgba(0,0,0,.125);
        padding: 1rem 1.25rem;
    }

    .badge {
        font-size: 0.875rem;
        padding: 0.5em 0.75em;
    }

    .btn-sm {
        padding: 0.25rem 0.75rem;
    }

    .list-group-item {
        transition: background-color 0.15s ease-in-out;
    }

    .list-group-item:hover {
        background-color: #f8f9fa;
    }
</style>