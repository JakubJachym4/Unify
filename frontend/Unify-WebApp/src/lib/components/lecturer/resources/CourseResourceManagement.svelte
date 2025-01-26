<script lang="ts">
import { onMount } from 'svelte';
import { 
    CreateCourseResource, 
    UpdateCourseResource, 
    DeleteCourseResource, 
	GetCourseResources

} from '$lib/api/Admin/Classes/CourseResourcesRequests';
import type { 
    CreateCourseResourceRequest, 
    UpdateCourseResourceRequest 
} from '$lib/api/Admin/Classes/CourseResourcesRequests';
import type { ApiRequestError } from '$lib/api/apiError';
	import { convertAttachmentsToFiles, ResourceType, type Attachment, type ClassResource } from '$lib/types/resources';
	
import { convertFilesToAttachments } from "$lib/types/resources";

export let courseId: string;
export const onBack = () => {};

let resources: ClassResource[] = [];
let error = '';
let loading = true;
let addingResource = false;
let editingResource: ClassResource | null = null;
let deletingResource: ClassResource | null = null;

let newResource = {
    title: '',
    description: '',
    attachments: null as File[] | null
};

let addForm: HTMLFormElement;
let editForm: HTMLFormElement;
let fileInput: HTMLInputElement;

const loadResources = async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) throw new Error('No token found');
        const data = await GetCourseResources(courseId, token);
        resources = data;
        loading = false;
    } catch (err) {
        error = (err as ApiRequestError).details;
        loading = false;
    }
};

const handleFileSelect = (event: Event, isEdit = false) => {
    const target = event.target as HTMLInputElement;
    const files = Array.from(target.files || []);
    if (isEdit && editingResource) {
        editingResource.attachments = convertFilesToAttachments(files)
    } else {
        newResource.attachments = files;
    }
};

const handleAdd = async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) throw new Error('No token found');
        await CreateCourseResource({
            ...newResource,
            courseId
        }, token);
        addingResource = false;
        newResource = { title: '', description: '', attachments: null };
        addForm.reset();
        await loadResources();
    } catch (err) {
        error = (err as ApiRequestError).details;
    }
};

const handleUpdate = async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) throw new Error('No token found');
        let attachments = editingResource ? convertAttachmentsToFiles(editingResource.attachments) : null;
        if(!editingResource) throw new Error('No resource found');

        await UpdateCourseResource({
            ...editingResource,
            attachments: attachments,
        }, token);
        editingResource = null;
        await loadResources();
    } catch (err) {
        error = (err as ApiRequestError).details;
    }
};

const handleDelete = async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) throw new Error('No token found');
        await DeleteCourseResource({ id: deletingResource!.id }, token);
        deletingResource = null;
        await loadResources();
    } catch (err) {
        error = (err as ApiRequestError).details;
    }
};

const downloadFile = (file: Attachment) => {
        const byteCharacters = atob(file.data);
        const byteNumbers = new Array(byteCharacters.length);
        for (let i = 0; i < byteCharacters.length; i++) {
            byteNumbers[i] = byteCharacters.charCodeAt(i);
        }
        const byteArray = new Uint8Array(byteNumbers);
        const blob = new Blob([byteArray], { type: file.contentType });
        
        const link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = file.fileName;
        link.click();
        window.URL.revokeObjectURL(link.href);
    };

onMount(loadResources);
</script>

<!-- Add HTML template -->
<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Course Resources</h2>
        <button class="btn btn-primary" on:click={() => addingResource = true}>
            Add Resource
        </button>
    </div>

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
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Attachments</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each resources as resource}
                        <tr>
                            <td>{resource.title}</td>
                            <td>{resource.description}</td>
                            <td>
                                {#if resource.attachments}
                                    {#each resource.attachments as attachment}
                                        <p>
                                            {attachment.fileName}
                                        </p>
                                    {/each}
                                {:else}
                                    <span class="text-muted">No attachments</span>
                                {/if}
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => editingResource = {...resource}}>
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => deletingResource = resource}>
                                    Delete
                                </button>
                            </td>
                        </tr>
                    {/each}
                </tbody>
            </table>
        </div>
    {/if}
</div>

<!-- Add Resource Modal -->
{#if addingResource}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form 
                    bind:this={addForm}
                    on:submit|preventDefault={async (e) => {
                        if (e.target.checkValidity()) {
                            await handleAdd();
                        }
                        e.target.classList.add('was-validated');
                    }}
                    class="needs-validation"
                    novalidate
                >
                    <div class="modal-header">
                        <h5 class="modal-title">Add New Resource</h5>
                        <button type="button" class="btn-close" on:click={() => addingResource = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={newResource.title}
                                required
                                minlength="2"
                            />
                            <div class="invalid-feedback">
                                Please enter a title (minimum 2 characters)
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control"
                                bind:value={newResource.description}
                                required
                                minlength="5"
                                rows="3"
                            ></textarea>
                            <div class="invalid-feedback">
                                Please enter a description (minimum 5 characters)
                            </div>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attachments</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={(e) => handleFileSelect(e)}
                                multiple
                                bind:this={fileInput}
                            />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => addingResource = false}
                        >
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Add Resource
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit Resource Modal -->
{#if editingResource}
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
                        <h5 class="modal-title">Edit Resource</h5>
                        <button type="button" class="btn-close" on:click={() => editingResource = null}></button>
                    </div>
                    <div class="modal-body">
                        <!-- Same form fields as Add Modal -->
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={editingResource.title}
                                required
                                minlength="2"
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control"
                                bind:value={editingResource.description}
                                required
                                minlength="5"
                                rows="3"
                            ></textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attachments</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={(e) => handleFileSelect(e, true)}
                                multiple
                            />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button 
                            type="button"
                            class="btn btn-secondary"
                            on:click={() => editingResource = null}
                        >
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">
                            Save Changes
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Delete Confirmation Modal -->
{#if deletingResource}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Confirm Delete</h5>
                    <button type="button" class="btn-close" on:click={() => deletingResource = null}></button>
                </div>
                <div class="modal-body">
                    <p>Are you sure you want to delete resource "{deletingResource.title}"?</p>
                </div>
                <div class="modal-footer">
                    <button 
                        class="btn btn-secondary"
                        on:click={() => deletingResource = null}
                    >
                        Cancel
                    </button>
                    <button 
                        class="btn btn-danger"
                        on:click={handleDelete}
                    >
                        Delete
                    </button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}