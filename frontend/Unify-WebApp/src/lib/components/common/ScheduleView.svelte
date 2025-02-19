<!-- filepath: /c:/Users/jakub/Downloads/Unify/frontend/Unify-WebApp/src/lib/components/shared/schedule/ScheduleView.svelte -->
<script lang="ts">
    import { onMount } from 'svelte';
    import type { ClassSession, HomeworkAssignment } from '$lib/types/resources';
    import type { AcademicLocation } from '$lib/types/university';
    import { GetAllLocations } from '$lib/api/Common/LocationRequests';
    import { createEventDispatcher } from 'svelte';
    import type { ApiRequestError } from '$lib/api/apiError';

    export let upcomingSessions: (ClassSession & { courseName?: string, className?: string })[] = [];
    export let upcomingLectures: (ClassSession & { courseName?: string } & {courseId: string})[] = [];
    export let upcomingAssignments: (HomeworkAssignment & { courseName?: string, className?: string })[] = [];

    const dispatch = createEventDispatcher<{
        viewSession: { classOfferingId: string };
        viewAssignment: { classOfferingId: string, assignmentId: string };
        viewLecture: { courseId: string };
    }>();

    let error = '';
    let loading = true;
    let locations: AcademicLocation[] = [];
    let viewType: 'day' | 'week' = 'week';
    let currentDate = new Date();

    const moveDate = (direction: 'prev' | 'next') => {
        const newDate = new Date(currentDate);
        const days = viewType === 'week' ? 7 : 1;
        newDate.setDate(newDate.getDate() + (direction === 'next' ? days : -days));
        currentDate = newDate;
    };

    const goToToday = () => {
        currentDate = new Date();
    };

    interface ScheduleEvent {
        id: string;
        title: string;
        courseName: string;
        startTime: Date;
        type: 'session' | 'lecture' | 'assignment';
        locationId?: string;
        duration?: string;
        classOfferingId?: string;
        courseId?: string;
        assignmentId?: string;
        locked?: boolean;
    }

    let events: ScheduleEvent[] = [];

    const loadLocations = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            locations = await GetAllLocations(token);
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const formatLocation = (locationId: string) => {
        const location = locations.find(l => l.id === locationId);
        if (!location) return 'Location not available';
        return location.online 
            ? 'Online Meeting' 
            : `${location.building}, Room ${location.doorNumber}`;
    };

    const getDaysInWeek = (date: Date) => {
        const week = [];
        const start = new Date(date);
        start.setDate(start.getDate() - start.getDay());

        for (let i = 0; i < 7; i++) {
            const day = new Date(start);
            day.setDate(day.getDate() + i);
            week.push(day);
        }
        return week;
    };

    $: {
        // Combine all events
        events = [
            ...upcomingSessions.map(session => ({
                id: session.id,
                title: session.title,
                courseName: session.courseName || '',
                startTime: new Date(session.scheduledDate),
                type: 'session' as const,
                locationId: session.locationId,
                duration: session.duration,
                classOfferingId: session.parentId
            })),
            ...upcomingLectures.map(lecture => ({
                id: lecture.id,
                title: lecture.title,
                courseName: lecture.courseName || '',
                startTime: new Date(lecture.scheduledDate),
                type: 'lecture' as const,
                locationId: lecture.locationId,
                duration: lecture.duration,
                courseId: lecture.courseId
            })),
            ...upcomingAssignments.map(assignment => ({
                id: assignment.id,
                title: assignment.title,
                courseName: assignment.courseName || '',
                startTime: new Date(assignment.dueDate),
                type: 'assignment' as const,
                classOfferingId: assignment.classOfferingId,
                assignmentId: assignment.id,
                locked: assignment.locked
            }))
        ].sort((a, b) => a.startTime.getTime() - b.startTime.getTime());
    }

    const handleEventClick = (event: ScheduleEvent) => {
        switch (event.type) {
            case 'session':
                if (event.classOfferingId) {
                    dispatch('viewSession', { classOfferingId: event.classOfferingId });
                }
                break;
            case 'lecture':
                if (event.courseId) {
                    dispatch('viewLecture', { courseId: event.courseId });
                }
                break;
            case 'assignment':
                if (event.classOfferingId && event.assignmentId) {
                    dispatch('viewAssignment', { 
                        classOfferingId: event.classOfferingId, 
                        assignmentId: event.assignmentId 
                    });
                }
                break;
        }
    };

    onMount(loadLocations);
</script>

<div class="schedule-container">
    <div class="card">
        <div class="card-header">
            <div class="d-flex justify-content-between align-items-center">
                <div class="d-flex align-items-center">
                    <h4 class="mb-0 me-3">Schedule</h4>
                    <div class="btn-group me-3">
                        <button 
                            class="btn btn-outline-secondary"
                            on:click={() => moveDate('prev')}
                            title={viewType === 'week' ? 'Previous Week' : 'Previous Day'}
                        >
                            <i class="bi bi-chevron-left">&lt;
                            </i>
                        </button>
                        <button 
                            class="btn btn-outline-secondary"
                            on:click={goToToday}
                        >
                            Today
                        </button>
                        <button 
                            class="btn btn-outline-secondary"
                            on:click={() => moveDate('next')}
                            title={viewType === 'week' ? 'Next Week' : 'Next Day'}
                        >
                            <i class="bi bi-chevron-right">
                                &gt;
                            </i>
                        </button>
                    </div>
                    <div class="current-period">
                        {#if viewType === 'week'}
                            {getDaysInWeek(currentDate)[0].toLocaleDateString('en-US', { 
                                month: 'short', 
                                day: 'numeric'
                            })} - {getDaysInWeek(currentDate)[6].toLocaleDateString('en-US', { 
                                month: 'short', 
                                day: 'numeric',
                                year: 'numeric'
                            })}
                        {:else}
                            {currentDate.toLocaleDateString('en-US', { 
                                weekday: 'long',
                                month: 'long',
                                day: 'numeric',
                                year: 'numeric'
                            })}
                        {/if}
                    </div>
                </div>
                <div class="btn-group">
                    <button 
                        class="btn {viewType === 'day' ? 'btn-primary' : 'btn-outline-primary'}"
                        on:click={() => viewType = 'day'}
                    >
                        Day
                    </button>
                    <button 
                        class="btn {viewType === 'week' ? 'btn-primary' : 'btn-outline-primary'}"
                        on:click={() => viewType = 'week'}
                    >
                        Week
                    </button>
                </div>
            </div>
        </div>
        <div class="card-body">
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
                <div class="schedule-grid">
                    {#if viewType === 'week'}
                        <div class="week-view">
                            {#each getDaysInWeek(currentDate) as day}
                                <div class="day-column">
                                    <div class="day-header">
                                        {day.toLocaleDateString('en-US', { weekday: 'short' })}
                                        <br>
                                        {day.toLocaleDateString('en-US', { day: 'numeric', month: 'short' })}
                                    </div>
                                    <div class="day-events">
                                        {#each events.filter(event => 
                                            event.startTime.toDateString() === day.toDateString()
                                        ) as event}
                                            <div 
                                                class="event-card {event.type}"
                                                on:click={() => handleEventClick(event)}
                                                role="button"
                                            >
                                                <div class="event-time">
                                                    {event.startTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                                </div>
                                                <div class="event-title">
                                                    {event.title}
                                                    {#if event.type === 'assignment' && event.locked}
                                                        <i class="bi bi-lock-fill"></i>
                                                    {/if}
                                                </div>
                                                <div class="event-course">{event.courseName}</div>
                                                {#if event.locationId}
                                                    <div class="event-location">
                                                        <i class="bi bi-geo-alt"></i> 
                                                        {formatLocation(event.locationId)}
                                                    </div>
                                                {/if}
                                            </div>
                                        {/each}
                                    </div>
                                </div>
                            {/each}
                        </div>
                    {:else}
                        <div class="day-view">
                            <div class="day-header">
                                {currentDate.toLocaleDateString('en-US', { 
                                    weekday: 'long', 
                                    year: 'numeric', 
                                    month: 'long', 
                                    day: 'numeric' 
                                })}
                            </div>
                            <div class="day-events">
                                {#each events.filter(event => 
                                    event.startTime.toDateString() === currentDate.toDateString()
                                ) as event}
                                    <div 
                                        class="event-card {event.type}"
                                        on:click={() => handleEventClick(event)}
                                        role="button"
                                    >
                                        <div class="event-time">
                                            {event.startTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                                        </div>
                                        <div class="event-title">
                                            {event.title}
                                            {#if event.type === 'assignment' && event.locked}
                                                <i class="bi bi-lock-fill"></i>
                                            {/if}
                                        </div>
                                        <div class="event-course">{event.courseName}</div>
                                        {#if event.locationId}
                                            <div class="event-location">
                                                <i class="bi bi-geo-alt"></i> 
                                                {formatLocation(event.locationId)}
                                            </div>
                                        {/if}
                                    </div>
                                {/each}
                            </div>
                        </div>
                    {/if}
                </div>
            {/if}
        </div>
    </div>
</div>

<style>
    .schedule-container {
        margin: 1rem 0;
    }

    .schedule-grid {
        overflow-x: auto;
    }

    .week-view {
        display: grid;
        grid-template-columns: repeat(7, 1fr);
        gap: 1rem;
        min-width: 768px;
    }

    .day-column {
        border: 1px solid #dee2e6;
        border-radius: 0.25rem;
    }

    .day-header {
        padding: 0.5rem;
        text-align: center;
        background-color: #f8f9fa;
        border-bottom: 1px solid #dee2e6;
    }

    .day-events {
        padding: 0.5rem;
        min-height: 200px;
    }

    .event-card {
        padding: 0.5rem;
        margin-bottom: 0.5rem;
        border-radius: 0.25rem;
        cursor: pointer;
        transition: transform 0.2s;
    }

    .event-card:hover {
        transform: translateY(-2px);
    }

    .event-card.session {
        background-color: rgba(var(--bs-primary-rgb), 0.1);
        border-left: 3px solid var(--bs-primary);
    }

    .event-card.lecture {
        background-color: rgba(var(--bs-info-rgb), 0.1);
        border-left: 3px solid var(--bs-info);
    }

    .event-card.assignment {
        background-color: rgba(var(--bs-danger-rgb), 0.1);
        border-left: 3px solid var(--bs-danger);
    }

    .event-time {
        font-size: 0.875rem;
        color: var(--bs-secondary);
    }

    .event-title {
        font-weight: 500;
        margin: 0.25rem 0;
    }

    .event-course {
        font-size: 0.875rem;
        color: var(--bs-secondary);
    }

    .event-location {
        font-size: 0.75rem;
        color: var(--bs-secondary);
        margin-top: 0.25rem;
    }

    @media (max-width: 768px) {
        .week-view {
            grid-template-columns: 1fr;
        }

        .day-column {
            margin-bottom: 1rem;
        }
    }
</style>