<script lang="ts">
	import LectureManagement from './LectureManagement.svelte';
    import { onMount } from 'svelte';
    import { courseStore } from '$lib/stores/course';
    import { GetCoursesByLecturer, UpdateCourse } from '$lib/api/Admin/Classes/CourseRequests';
    import type { Course } from '$lib/types/universityClasses';
    import type { ApiRequestError } from '$lib/api/apiError';
    import { user } from '$lib/stores/user';
    import { get } from 'svelte/store';
	import { specializationsStore } from '$lib/stores/university';
	import { getAllSpecializations } from '$lib/api/Admin/Specialization/SpecializationRequests';
	import { globalUsers } from '$lib/stores/globalUsers';
    import ClassOfferingManagement from './ClassOfferingManagement.svelte';
	import CourseResourceManagement from './resources/CourseResourceManagement.svelte';

    let courses: Course[] = [];
    let specializations = [];
    let error = '';
    let loading = true;
    let searchTerm = '';
    let editingCourse: Course | null = null;
    let editForm: HTMLFormElement;
    let updating = false;
    let selectedCourse: Course | null = null;
    let editingResources: Course | null = null;
    let lectureCourse: Course | null = null;

    const loadCourses = async () => {
        try {
            const $courses = get(courseStore);
            if($courses.length > 0 && !updating) {
                courses = $courses;
                loading = false;
                return;
            }

            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const fetchedCourses = await GetCoursesByLecturer($user!.id, token);
            const fetchedSpecializations = await getAllSpecializations(token); 
            const users = get(globalUsers);
            const coursesWithDetails = fetchedCourses.map(course => ({
                ...course,
                specialization: fetchedSpecializations.find(specialization => specialization.id === course.specializationId)!,
                lecturer: users.find(user => user.id === course.lecturerId)!,
                classOfferings: course.classOfferings?.map(classOffering => ({
                    ...classOffering,
                    lecturer: users.find(user => user.id === classOffering.lecturerId)!
                })) ?? []
            }));
            courseStore.set(coursesWithDetails);
            loading = false;
            updating = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
            updating = false;
        }
    };

    const handleUpdate = async () => {
        if (!editingCourse) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await UpdateCourse({
                id: editingCourse.id,
                name: editingCourse.name,
                description: editingCourse.description
            }, token);
            editingCourse = null;
            updating = true;
            loadCourses();

        } catch (err) {
            console.log(err);
            error = (err as ApiRequestError).details;
        }
    };

    const handleCourseSelect = (course: Course) => {
        selectedCourse = course;
        selectedCourse.classOfferings = selectedCourse.classOfferingResponses.map(classOffering => ({
            ...classOffering,
        }));
    };

    $: filteredCourses = courses.filter(course => {
        const searchLower = searchTerm.toLowerCase();
        return course.name.toLowerCase().includes(searchLower) ||
               course.description.toLowerCase().includes(searchLower);
    });

    onMount(() => {
        const unsubscribeSpecializations = specializationsStore.subscribe(value => {
            specializations = value;
        });

        const unsubscribe = courseStore.subscribe(value => {
            courses = value;
        });
        loadCourses();
        return () => {
            unsubscribe();
            unsubscribeSpecializations();
        };
    });
</script>

<div class="container mt-4">
    {#if !selectedCourse && !editingResources && !lectureCourse}
    <h1 class="mb-4">My Courses</h1>
    {/if}

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
    {#if !selectedCourse && !editingResources && !lectureCourse}
        <div class="row mb-3">
            <div class="col">
                <input
                    type="search"
                    class="form-control"
                    placeholder="Search courses..."
                    bind:value={searchTerm}
                />
            </div>
        </div>
        {/if}

        {#if selectedCourse}
        {#key updating}
            <ClassOfferingManagement 
            course={selectedCourse}
            onBack={() => selectedCourse = null}
            on:refresh={() => {updating = true; loadCourses()}}
        />
        {/key}

        {:else if editingResources}
            <CourseResourceManagement 
                courseId={editingResources.id}
                onBack={() => editingResources = null} />
            
        {:else if lectureCourse}
            <LectureManagement 
            courseId={lectureCourse.id}
            onBack={() => lectureCourse = null}/>
        {:else}
            <div class="table-responsive">
                <table class="table">
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Description</th>
                            <th>Actions</th>
                        </tr>
                    </thead>
                    <tbody>
                        {#each filteredCourses as course}
                            <tr>
                                <td>{course.name}</td>
                                <td>{course.description}</td>
                                <td>
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => editingCourse = {...course}}>
                                        Edit
                                    </button>
                                    <button 
                                        class="btn btn-sm btn-outline-secondary"
                                        on:click={() => handleCourseSelect(course)}>
                                        Manage Classes
                                    </button>
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => editingResources = course}>
                                        Edit Resources
                                    </button>
                                    <button 
                                        class="btn btn-sm btn-outline-primary"
                                        on:click={() => lectureCourse = course}>
                                        Lectures
                                    </button>
                                </td>
                            </tr>
                        {/each}
                    </tbody>
                </table>
            </div>
        {/if}
    {/if}
</div>

<!-- Edit Modal -->
{#if editingCourse}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    bind:this={editForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleUpdate();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Course</h5>
                        <button 
                            type="button" 
                            class="btn-close" 
                            on:click={() => editingCourse = null}
                        ></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Name</label>
                            <input 
                                class="form-control"
                                bind:value={editingCourse.name}
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a course name (minimum 2 characters)
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control"
                                bind:value={editingCourse.description}
                                required
                                minlength="5"
                                rows="3"
                            ></textarea>
                            <div class="invalid-feedback">
                                Please enter a description (minimum 5 characters)
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingCourse = null}
                        >
                            Cancel
                        </button>
                        <button 
                            type="submit"
                            class="btn btn-primary"
                        >
                            Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}