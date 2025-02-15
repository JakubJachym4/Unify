<script lang="ts">
    import { onMount } from 'svelte';
    import { 
        CreateClassOfferingResource, 
        UpdateClassOfferingResource, 
        DeleteClassOfferingResource, 
        GetClassOfferingResources
    } from '$lib/api/Admin/Classes/ClassOfferingResourcesRequests';
    import type { 
        CreateClassOfferingResourceRequest, 
        UpdateClassOfferingResourceRequest 
    } from '$lib/api/Admin/Classes/ClassOfferingResourcesRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { Attachment, ClassResource } from '$lib/types/resources';
    import { convertAttachmentsToFiles, convertFilesToAttachments, ResourceType } from '$lib/types/resources';
    
    export let classOfferingId: string;
    export let onBack: () => void;

    let resources: ClassResource[] = [];
    let error = '';
    let loading = true;
    let addingResource = false;
    let editingResourceState = false;
    let addForm: HTMLFormElement;
    let fileInput: HTMLInputElement;
        
    let newResource = {
        title: '',
        description: '',
        attachments: null as File[] | null
    };

    let editingResource = {
        id: '',
        title: '',
        description: '',
        attachments: null as File[] | null
    };

    const loadResources = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            const data = await GetClassOfferingResources(classOfferingId, token);
            resources = data;
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const handleAdd = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await CreateClassOfferingResource({
                ...newResource,
                classOfferingId: classOfferingId
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
            
            
            const updateData = {
            ...editingResource,
            attachments: editingResource.attachments
                 };


            await UpdateClassOfferingResource({
                ...updateData
            }, token);
            editingResource = { id: '', title: '', description: '', attachments: null };
            editingResourceState = false;
            await loadResources();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteClassOfferingResource({ id }, token);
            await loadResources();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleFileSelect = (event: Event, isEdit = false) => {
        const target = event.target as HTMLInputElement;
        const files = Array.from(target.files || []);
        if (isEdit) {
            editingResource.attachments = files;
        } else {
            newResource.attachments = files;
        }
    };

    const isImage = (file: Attachment) => {
        return file.contentType.startsWith('image/');
    };

    const getFileIcon = (fileName: string) => {
        const extension = fileName.split('.').pop()?.toLowerCase();
        switch(extension) {
            case 'pdf': return 'file-pdf';
            case 'doc':
            case 'docx': return 'file-word';
            case 'xls':
            case 'xlsx': return 'file-excel';
            case 'ppt':
            case 'pptx': return 'file-powerpoint';
            default: return 'file';
        }
    };

    onMount(loadResources);
</script>

    <div class="container mt-4">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2>Class Resources</h2>
            <div>
                <button class="btn btn-primary me-2" on:click={() => addingResource = true}>
                    Add Resource
                </button>
                <button class="btn btn-secondary" on:click={onBack}>
                    Back
                </button>
            </div>
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
                                    <div class="d-flex flex-wrap gap-2">
                                        {#each resource.attachments as attachment}
                                            <div class="attachment-item">
                                                {#if isImage(attachment)}
                                                    <img 
                                                        src={`data:${attachment.contentType};base64,${attachment.data}`} 
                                                        alt={attachment.fileName}
                                                        style="max-width: 100px; max-height: 100px;"
                                                        class="attachment-preview"
                                                    />
                                                {:else}
                                                    <i class="bi bi-{getFileIcon(attachment.fileName)} fs-2"></i>
                                                {/if}
                                                <div class="mt-1">
                                                    <small class="d-block text-truncate" style="max-width: 100px;">
                                                        {attachment.fileName}
                                                    </small>
                                                    <a 
                                                        href={`data:${attachment.contentType};base64,${attachment.data}`}
                                                        download={attachment.fileName}
                                                        class="btn btn-sm btn-outline-primary mt-1"
                                                    >
                                                        <i class="bi bi-download"></i> Download
                                                    </a>
                                                </div>
                                            </div>
                                        {/each}
                                    </div>
                                    {:else}
                                        <span class="text-muted">No attachments</span>
                                    {/if}
                                </td>
                                <td>
                                    <button 
                                        class="btn btn-sm btn-outline-primary me-2"
                                        on:click={() => {
                                            editingResource = {...resource, attachments: convertAttachmentsToFiles(resource.attachments)};
                                            editingResourceState = true;
                                        }}
                                    >
                                        Edit
                                    </button>
                                    <button 
                                        class="btn btn-sm btn-outline-danger"
                                        on:click={() => handleDelete(resource.id)}
                                    >
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
                        on:submit|preventDefault={handleAdd}
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
    {#if editingResourceState}
        <div class="modal fade show" style="display: block;">
            <div class="modal-dialog">
                <div class="modal-content">
                    <form 
                        on:submit|preventDefault={handleUpdate}
                        class="needs-validation"
                        novalidate
                    >
                        <div class="modal-header">
                            <h5 class="modal-title">Edit Resource</h5>
                            <button type="button" class="btn-close" on:click={() => {
                                editingResource = {
                                    id: '',
                                    title: '',
                                    description: '',
                                    attachments: null
                                };
                                editingResourceState = false;
                            }}></button>
                        </div>
                        <div class="modal-body">
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
                                on:click={() => {
                                    editingResource = {
                                        id: '',
                                        title: '',
                                        description: '',
                                        attachments: null
                                    };
                                    editingResourceState = false;
                                }}
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

<style>
    .attachment-item {
        border: 1px solid #dee2e6;
        border-radius: 0.25rem;
        padding: 0.5rem;
        text-align: center;
    }

    .attachment-preview {
        object-fit: cover;
        border-radius: 0.25rem;
    }
</style>