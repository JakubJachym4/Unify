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
	import { getUserData } from '$lib/api/User/UserRequests';
	import { get } from 'svelte/store';
    import { GetLecturesByCourse } from '$lib/api/Admin/Classes/LectureRequests';

    let loading = true;
    let error = '';
    let upcomingSessions: (ClassSession & { courseName?: string, className?: string })[] = [];
    let upcomingAssignments: (HomeworkAssignment & { courseName?: string, className?: string })[] = [];
    let upcomingLectures: (ClassSession & { courseName?: string } & {courseId: string})[] = [];
    let retried = false;

    const DAYS_TO_SHOW = 7; // Show items for next 7 days

    const dispatch = createEventDispatcher<{
        openSession: { classOfferingId: string };
        openAssignmentSubmissions: { classOfferingId: string, selectedAssignmentId: string };
        openLecture: { courseId: string };
    }>();

    const loadDashboardData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            if($user == null){
                const userData = await getUserData(token);
                user.set(userData);
            }

            const offerings = await GetClassOfferingsByLecturer($user!.id, token);

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

            const uniqueCourseIds = [...new Set(offerings.map(o => o.courseId))];

            const lecturesPromises = uniqueCourseIds.map(async (courseId) => {
                const lectures = await GetLecturesByCourse(courseId, token);
                const course = await GetCourseById(courseId, token);
                return lectures.map(lecture => ({
                    ...lecture,
                    courseName: course.name,
                    courseId: course.id
                }));
            });

            const [allSessions, allAssignments, allLectures] = await Promise.all([
                Promise.all(sessionsPromises).then(results => results.flat()),
                Promise.all(assignmentsPromises).then(results => results.flat()),
                Promise.all(lecturesPromises).then(results => results.flat())
            ]);

            const now = new Date();
            const futureDate = new Date();
            futureDate.setDate(now.getDate() + DAYS_TO_SHOW);

            upcomingSessions = allSessions
                .filter(session => {
                    const sessionDate = new Date(session.scheduledDate);
                    return sessionDate >= now && sessionDate <= futureDate;
                })
                .sort((a, b) => new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime());

            upcomingAssignments = allAssignments
                .filter(assignment => {
                    const dueDate = new Date(assignment.dueDate);
                    return dueDate >= now && dueDate <= futureDate;
                })
                .sort((a, b) => new Date(a.dueDate).getTime() - new Date(b.dueDate).getTime());

            upcomingLectures = allLectures
                .filter(lecture => {
                    const lectureDate = new Date(lecture.scheduledDate);
                    return lectureDate >= now && lectureDate <= futureDate;
                })
                .sort((a, b) => new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime());

            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const formatDateTime = (date: string): string => {
        return new Date(date).toLocaleString();
    };

    const cutMinutes = (date: string): string => {
        return date.split('.')[0]
    };

    const handleSessionClick = (classOfferingId: string) => {
        dispatch('openSession', { classOfferingId });
    };

    const handleAssignmentClick = (classOfferingId: string, selectedSubmissionId: string) => {
        dispatch('openAssignmentSubmissions', { classOfferingId, selectedAssignmentId: selectedSubmissionId});
    };

    const handleLectureClick = (courseId: string) => {
    dispatch('openLecture', { courseId });
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
            <!-- Upcoming Classes -->
            <div class="col-md-4 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Upcoming Classes</h5>
                    </div>
                    <div class="card-body">
                        {#if upcomingSessions.length === 0}
                            <p class="text-muted">No upcoming classes in the next {DAYS_TO_SHOW} days</p>
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
                                            <small class="text-muted">Duration: {cutMinutes(session.duration)} minutes</small>
                                        </div>
                                    </div>
                                {/each}
                            </div>
                        {/if}
                    </div>
                </div>
            </div>

            <div class="col-md-4 mb-4">
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
                                        on:click={() => handleAssignmentClick(assignment.classOfferingId, assignment.id)}
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
                                    
                                    </div>
                                {/each}
                            </div>
                        {/if}
                    </div>
                </div>
            </div>

            <!-- Upcoming Lectures -->
            <div class="col-md-4 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">Upcoming Lectures</h5>
                    </div>
                    <div class="card-body">
                        {#if upcomingLectures.length === 0}
                            <p class="text-muted">No upcoming lectures in the next {DAYS_TO_SHOW} days</p>
                        {:else}
                            <div class="list-group">
                                <!-- svelte-ignore a11y_interactive_supports_focus -->
                                <!-- svelte-ignore a11y_click_events_have_key_events -->
                                <!-- svelte-ignore a11y_interactive_supports_focus -->
                                {#each upcomingLectures as lecture}
                                    <div 
                                        class="list-group-item list-group-item-action dashboard-item"
                                        role="button"
                                        on:click={() => handleLectureClick(lecture.courseId)}
                                    >
                                        <div class="d-flex w-100 justify-content-between align-items-start mb-2">
                                            <h5 class="mb-1">{lecture.title}</h5>
                                            <span class="badge bg-info rounded-pill">
                                                {formatDateTime(lecture.scheduledDate)}
                                            </span>
                                        </div>
                                        <h6 class="mb-2">{lecture.courseName}</h6>
                                        <div class="d-flex justify-content-between align-items-center">
                                            <small class="text-muted">Duration: {cutMinutes(lecture.duration)} minutes</small>
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

    .list-group-item.dashboard-item:has(.bg-info) {
        border-left: 4px solid var(--bs-info);
    }
</style>