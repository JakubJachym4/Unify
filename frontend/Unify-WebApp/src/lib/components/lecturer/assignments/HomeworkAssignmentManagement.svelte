<script lang="ts">
    import { onMount } from 'svelte';
    import { 
        CreateHomeworkAssignment,
        UpdateHomeworkAssignment,
        DeleteHomeworkAssignment,
        GradeHomeworkSubmission,
        GetHomeworkAssignmentsByClassOfferingId
    } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { 
        CreateHomeworkAssignmentRequest,
        UpdateHomeworkAssignmentRequest,
        GradeHomeworkSubmissionRequest
    } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { HomeworkAssignment, HomeworkSubmission, Attachment } from '$lib/types/resources';
    import { convertFilesToAttachments, convertAttachmentsToFiles } from '$lib/types/resources';
	import HomeworkSubmissionManagement from './HomeworkSubmissionManagement.svelte';
    import { type Grade } from '$lib/types/universityClasses';
    
    export let classOfferingId: string;
    export let onBack: () => void;

    let assignments: HomeworkAssignment[] = [];
    let error = '';
    let loading = true;
    let addingAssignment = false;
    let gradingSubmission: HomeworkSubmission | null = null;
    let editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: null as string | null,
                            dueDate: '',
                            attachments: null as File[] | null
                        }
    let editingAssignmentState = false;
    
    let submissionAssignment: HomeworkAssignment | null = null;

    let newAssignment: CreateHomeworkAssignmentRequest = {
        classOfferingId: classOfferingId,
        title: '',
        description: '',
        criteria: null as string | null,
        dueDate: '',
        attachments: null
    };

    const loadAssignments = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            assignments = await GetHomeworkAssignmentsByClassOfferingId(classOfferingId, token);
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
            await CreateHomeworkAssignment(newAssignment, token);
            addingAssignment = false;
            newAssignment = {
                classOfferingId,
                title: '',
                description: '',
                criteria: '',
                dueDate: '',
                attachments: null
            };
            await loadAssignments();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async () => {
        if (!editingAssignment) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');


            const updateRequest: UpdateHomeworkAssignmentRequest = {
                id: editingAssignment.id,
                title: editingAssignment.title,
                description: editingAssignment.description,
                criteria: editingAssignment.criteria,
                dueDate: editingAssignment.dueDate,
                attachments: editingAssignment.attachments
            };
            await UpdateHomeworkAssignment(updateRequest, token);
            editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: '',
                            dueDate: '',
                            attachments: null as File[] | null
                        }
    
            await loadAssignments();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async (id: string) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await DeleteHomeworkAssignment({ id }, token);
            await loadAssignments();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleGradeSubmission = async (formData: GradeHomeworkSubmissionRequest) => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            await GradeHomeworkSubmission(formData, token);
            gradingSubmission = null;
            await loadAssignments();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };


    const handleFileSelect = (event: Event, isEdit = false) => {
        const target = event.target as HTMLInputElement;
        const files = Array.from(target.files || []);
        if (isEdit && editingAssignment) {
            editingAssignment = {
                ...editingAssignment,
                attachments: files
            };
        } else {
            newAssignment.attachments = files;
        }
    };

    const isImage = (attachment: Attachment) => {
        return attachment.contentType.startsWith('image/');
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

    onMount(loadAssignments);
</script>

<div class="container mt-4">
    {#if !submissionAssignment}
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Homework Assignments</h2>
        <div>
            <button class="btn btn-primary me-2" on:click={() => addingAssignment = true}>
                Add Assignment
            </button>
            <button class="btn btn-secondary" on:click={onBack}>
                Back
            </button>
        </div>
    </div>
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
    {:else if submissionAssignment}
    <HomeworkSubmissionManagement
        assignmentId={submissionAssignment.id}
        onBack={() => submissionAssignment = null}
        classOfferingId={classOfferingId}/>

    {:else}
        <div class="table-responsive">
            <table class="table">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Description</th>
                        <th>Criteria</th>
                        <th>Due Date</th>
                        <th>Attachments</th>
                        <th>Submissions</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {#each assignments as assignment}
                        <tr>
                            <td>{assignment.title}</td>
                            <td>{assignment.description}</td>
                            <td>{assignment.criteria}</td>
                            <td>{new Date(assignment.dueDate).toLocaleString()}</td>
                            <td>
                                {#if assignment.attachments?.length}
                                    <div class="d-flex flex-wrap gap-2">
                                        {#each assignment.attachments as attachment}
                                            <div class="attachment-item">
                                                {#if isImage(attachment)}
                                                    <img 
                                                        src={`data:${attachment.contentType};base64,${attachment.data}`}
                                                        alt={attachment.fileName}
                                                        class="attachment-preview"
                                                        style="max-width: 50px; max-height: 50px;"
                                                    />
                                                {:else}
                                                    <i class="bi bi-{getFileIcon(attachment.fileName)} fs-2"></i>
                                                {/if}
                                                <div class="mt-1">
                                                    <small class="d-block text-truncate" style="max-width: 75px;">
                                                        {attachment.fileName}
                                                    </small>
                                                    <a 
                                                        href={`data:${attachment.contentType};base64,${attachment.data}`}
                                                        download={attachment.fileName}
                                                        class="btn btn-sm btn-outline-primary mt-1"
                                                    >
                                                        <i class="bi bi-download">Download</i>
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
                                    class="btn btn-sm btn-outline-info"
                                    on:click={() => { submissionAssignment = assignment; }}
                                >
                                    View Submissions
                                </button>
                            </td>
                            <td>
                                <button 
                                    class="btn btn-sm btn-outline-primary me-2"
                                    on:click={() => {editingAssignment = {...assignment, attachments: convertAttachmentsToFiles(assignment.attachments)}; editingAssignmentState = true;}}
                                >
                                    Edit
                                </button>
                                <button 
                                    class="btn btn-sm btn-outline-danger"
                                    on:click={() => handleDelete(assignment.id)}
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

<!-- Add Assignment Modal -->
{#if addingAssignment}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form on:submit|preventDefault={handleAdd}>
                    <div class="modal-header">
                        <h5 class="modal-title">Add New Assignment</h5>
                        <button type="button" class="btn-close" on:click={() => addingAssignment = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={newAssignment.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control"
                                bind:value={newAssignment.description}
                                required
                                rows="3"
                            ></textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Criteria</label>
                            <textarea 
                                class="form-control"
                                bind:value={newAssignment.criteria}
                                required
                                rows="3"
                            ></textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Due Date</label>
                            <input 
                                type="date"
                                class="form-control"
                                bind:value={newAssignment.dueDate}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Attachments</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={(e) => handleFileSelect(e)}
                                multiple
                            />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" on:click={() => addingAssignment = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">Add Assignment</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit Assignment Modal -->
{#if editingAssignment && editingAssignmentState}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form on:submit|preventDefault={handleUpdate}>
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Assignment</h5>
                        <button type="button" class="btn-close" on:click={() => {editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: null as string | null,
                            dueDate: '',
                            attachments: null as File[] | null
                        }
                        editingAssignmentState = false;}}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Title</label>
                            <input 
                                class="form-control"
                                bind:value={editingAssignment.title}
                                required
                            />
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Description</label>
                            <textarea 
                                class="form-control"
                                bind:value={editingAssignment.description}
                                required
                                rows="3"
                            ></textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Criteria</label>
                            <textarea 
                                class="form-control"
                                bind:value={editingAssignment.criteria}
                                required
                                rows="3"
                            ></textarea>
                        </div>
                        <div class="mb-3">
                            <label class="form-label">Due Date</label>
                            <input 
                                type="date"
                                class="form-control"
                                bind:value={editingAssignment.dueDate}
                                required
                            />
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
                        <button type="button" class="btn btn-secondary" on:click={() => {editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: null as string | null,
                            dueDate: '',
                            attachments: null as File[] | null
                        }
                        editingAssignmentState = false;}}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">Update Assignment</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<style>
    .table td {
        vertical-align: middle;
    }
</style>