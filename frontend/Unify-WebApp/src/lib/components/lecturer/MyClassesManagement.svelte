<script lang="ts">
    import { onMount } from 'svelte';
    import { GetClassOfferingsByLecturer } from '$lib/api/Admin/Classes/ClassOfferingsRequests';
    import { GetAllLocations } from '$lib/api/Common/LocationRequests';
    import type { ClassOffering, Course } from '$lib/types/universityClasses';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { AcademicLocation } from '$lib/types/university';
    import { user } from '$lib/stores/user';
    import { courseStore } from '$lib/stores/course';
    import { GetCourseById, type CourseResponse } from '$lib/api/Admin/Classes/CourseRequests';

    let classOfferings: (ClassOffering & { course?: CourseResponse })[] = [];
    let locations: AcademicLocation[] = [];
    let error = '';
    let loading = true;
    let searchTerm = '';

    const loadClassOfferings = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            const offerings = await GetClassOfferingsByLecturer($user!.id, token);
            
            // Fetch course details for each offering
            const offeringsWithCourses = await Promise.all(
                offerings.map(async (offering) => {
                    const course = await GetCourseById(offering.courseId, token);
                    return {
                        ...offering,
                        course
                    };
                })
            );
            
            classOfferings = offeringsWithCourses;
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

    $: filteredOfferings = classOfferings.filter(offering => {
        const searchLower = searchTerm.toLowerCase();
        return offering.name.toLowerCase().includes(searchLower) ||
               offering.course?.name.toLowerCase().includes(searchLower);
    });

    onMount(async () => {
        await Promise.all([loadClassOfferings(), loadLocations()]);
    });
</script>

<div class="container mt-4">
    <h2 class="mb-4">My Classes</h2>

    {#if error}
        <div class="alert alert-danger" role="alert">{error}</div>
    {/if}

    <div class="row mb-3">
        <div class="col">
            <input
                type="search"
                class="form-control"
                placeholder="Search classes..."
                bind:value={searchTerm}
            />
        </div>
    </div>

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
                        <th>Course</th>
                        <th>Class Name</th>
                        <th>Duration</th>
                        <th>Student Group</th>
                        <th>Schedule</th>
                    </tr>
                </thead>
                <tbody>
                    {#each filteredOfferings as offering}
                        {@const startDate = new Date(offering.startDate)}
                        {@const endDate = new Date(offering.endDate)}
                        <tr>
                            <td>
                                {offering.course?.name || 'Unknown Course'}
                                <br>
                                <small class="text-muted">{offering.course?.description}</small>
                            </td>
                            <td>{offering.name}</td>
                            <td>
                                {startDate.toLocaleDateString()} - {endDate.toLocaleDateString()}
                            </td>
                            <td>
                                {#if offering.studentGroupId}
                                    {offering.studentGroupId}
                                {:else}
                                    <span class="text-muted">No group assigned</span>
                                {/if}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary"
                                    on:click={() => {/* Add session management navigation */}}
                                >
                                    View Sessions
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
    .table td {
        vertical-align: middle;
    }
</style>