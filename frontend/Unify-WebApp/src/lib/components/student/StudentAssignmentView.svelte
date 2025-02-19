<script lang="ts">
    import { onMount } from 'svelte';
    import { 
        GetHomeworkAssignmentById,
    } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import {
        CreateHomeworkSubmission,
        UpdateHomeworkSubmission,
        GetHomeworkSubmissionsByStudent,
        DeleteHomeworkSubmission
    } from '$lib/api/Admin/Assignments/HomeworkSubmissionsRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { HomeworkAssignment, HomeworkSubmission, Attachment } from '$lib/types/resources';
    import { convertFilesToAttachments, convertAttachmentsToFiles } from '$lib/types/resources';
    import { user } from '$lib/stores/user';
    import type { Grade, Mark } from '$lib/types/universityClasses';

    export let assignmentId: string;
    export let onBack: () => void;

    let assignment: HomeworkAssignment | null = null;
    let submission: HomeworkSubmission | null = null;
    let error = '';
    let loading = true;
    let submitting = false;
    let editingSubmission = false;
    let editingFiles: File[] | null = null;

    let newSubmission = {
        assignmentId: assignmentId,
        attachments: null as File[] | null
    };

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            // Load assignment and existing submission in parallel
            const [assignmentData, submissions] = await Promise.all([
                GetHomeworkAssignmentById(assignmentId, token),
                GetHomeworkSubmissionsByStudent($user!.id, token)
            ]);

            assignment = assignmentData;
            submission = submissions.find(s => s.assignmentId === assignmentId) || null;
            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const handleSubmit = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            await CreateHomeworkSubmission({
                assignmentId,
                attachments: newSubmission.attachments
            }, token);

            await loadData(); // Reload to get the new submission
            submitting = false;
            newSubmission.attachments = null;
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleUpdate = async () => {
        if (!submission) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            console.log(submission.attachments);

            await UpdateHomeworkSubmission({
                id: submission.id,
                attachments: editingFiles
            }, token);

            editingSubmission = false;
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleDelete = async () => {
        if (!submission || !confirm('Are you sure you want to delete your submission?')) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            await DeleteHomeworkSubmission({ id: submission.id }, token);
            submission = null;
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleFileSelect = (event: Event) => {
        const target = event.target as HTMLInputElement;
        const files = Array.from(target.files || []);
        if(editingSubmission && submission){
            editingFiles = files;
            return;
        }
        newSubmission.attachments = files;
    };

    const isImage = (file: Attachment) => file.contentType.startsWith('image/');
    
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

    const formatDateTime = (dateString: string): string => {
    try {
        const date = new Date(dateString);
        if (isNaN(date.getTime())) {
            throw new Error('Invalid date');
        }
        return date.toLocaleString();
    } catch (error) {
        return 'Date not available';
    }
};

    onMount(loadData);
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Assignment Details</h2>
        <button class="btn btn-secondary" on:click={onBack}>Back</button>
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
    {:else if assignment}
        <!-- Assignment Details -->
        <div class="card mb-4">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h3 class="card-title mb-0">{assignment.title}</h3>
                {#if assignment.locked}
                    <span class="badge bg-warning">
                        <i class="bi bi-lock-fill"></i> Locked
                    </span>
                {/if}
            </div>
            <div class="card-body">
                <p class="card-text">{assignment.description}</p>
                <p class="card-text">Grading Criteria: {assignment.description ?? "Not Defined"}</p>
                <p class="card-text">
                    <strong>Due Date:</strong> 
                    <span class={new Date(assignment.dueDate) < new Date() ? 'text-danger' : ''}>
                        {new Date(assignment.dueDate).toLocaleString()}
                    </span>
                </p>
                
                {#if assignment.attachments?.length}
                    <div class="mt-3">
                        <h5>Assignment Materials:</h5>
                        <div class="d-flex flex-wrap gap-2">
                            {#each assignment.attachments as attachment}
                                <div class="attachment-item">
                                    {#if isImage(attachment)}
                                        <img 
                                            src={`data:${attachment.contentType};base64,${attachment.data}`}
                                            alt={attachment.fileName}
                                            class="attachment-preview"
                                        />
                                    {:else}
                                        <i class="bi bi-{getFileIcon(attachment.fileName)} fs-2"></i>
                                    {/if}
                                    <small class="d-block text-truncate">{attachment.fileName}</small>
                                    <a 
                                        href={`data:${attachment.contentType};base64,${attachment.data}`}
                                        download={attachment.fileName}
                                        class="btn btn-sm btn-outline-primary mt-1"
                                    >
                                        <i class="bi bi-download">Download</i>
                                    </a>
                                </div>
                            {/each}
                        </div>
                    </div>
                {/if}
            </div>
        </div>

        <!-- Submission Section -->
        <div class="card">
            <div class="card-header">
                <h4 class="mb-0">Your Submission</h4>
            </div>
            <div class="card-body">
                {#if submission}
                    <!-- Replace the existing grade section with this updated version -->
                    {#if submission?.mark}
                        <div class="alert alert-info mb-3">
                            <div class="d-flex justify-content-between align-items-start mb-2">
                                <h5 class="mb-0">Mark: {submission.mark.score}%</h5>
                                <small class="text-muted">
                                    Awarded on: {formatDateTime(submission.mark.dateAwarded)}
                                </small>
                            </div>
                            
                            {#if submission.mark}
                                <div class="mt-3">
                                    <h6>Detailed Assessment:</h6>
                                    <div class="table-responsive">
                                        <table class="table table-sm">
                                            <thead>
                                                <tr>
                                                    <th class="text-end">Score</th>
                                                    <th class="text-end">Max Score</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                
                                                <tr>
                                                    <td class="text-end">{submission.mark.score}</td>
                                                    <td class="text-end">{submission.mark.maxScore}</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    <h5>Feedback:</h5>
                                    <h5>{submission.feedback}</h5>
                                    </div>
                                </div>
                            {/if}
                        </div>
                    {/if}

                    <p>Submitted on: {formatDateTime(submission.submittedOn)}</p>
                    
                    {#if submission.attachments?.length}
                        <div class="mt-3">
                            <h5>Submitted Files:</h5>
                            <div class="d-flex flex-wrap gap-2">
                                {#each submission.attachments as attachment}
                                    <div class="attachment-item">
                                        {#if isImage(attachment)}
                                            <img 
                                                src={`data:${attachment.contentType};base64,${attachment.data}`}
                                                alt={attachment.fileName}
                                                class="attachment-preview"
                                            />
                                        {:else}
                                            <i class="bi bi-{getFileIcon(attachment.fileName)} fs-2"></i>
                                        {/if}
                                        <small class="d-block text-truncate">{attachment.fileName}</small>
                                        <a 
                                            href={`data:${attachment.contentType};base64,${attachment.data}`}
                                            download={attachment.fileName}
                                            class="btn btn-sm btn-outline-primary mt-1"
                                        >
                                            <i class="bi bi-download">Download</i>
                                        </a>
                                    </div>
                                {/each}
                            </div>
                        </div>
                    {/if}

                    <!-- Update the submission modification check -->
                    <div class="mt-3">
                        {#if !submission?.mark}
                            <button 
                                class="btn btn-outline-primary me-2"
                                on:click={() => editingSubmission = true}
                            >
                                Edit Submission
                            </button>
                            <button 
                                class="btn btn-outline-danger"
                                on:click={handleDelete}
                            >
                                Delete Submission
                            </button>
                        {:else}
                            <div class="text-muted">
                                <i class="bi bi-lock"></i> 
                                Submission has been graded and cannot be modified
                            </div>
                        {/if}
                    </div>
                {:else}
                    <div class="text-danger">
                        <h5 class="bi bi-lock">
                            Submission not send!
                        </h5> 
                        
                    </div>
                    {#if !assignment.locked}
                        <button 
                            class="btn btn-primary"
                            on:click={() => submitting = true}
                            disabled={new Date(assignment.dueDate) < new Date()}
                        >
                            Submit Assignment
                        </button>
                    {:else}
                        <div class="text-muted">
                            <i class="bi bi-lock"></i> 
                            Assignment has been locked and cannot be modified.
                        </div>
                    
                        {#if new Date(assignment.dueDate) < new Date()}
                            <div class="text-danger mt-2">
                                Assignment deadline has passed
                            </div>
                        {/if}
                    {/if}
                    
                {/if}
            </div>
        </div>
    {/if}
</div>

<!-- Submit Modal -->
{#if submitting}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form on:submit|preventDefault={handleSubmit}>
                    <div class="modal-header">
                        <h5 class="modal-title">Submit Assignment</h5>
                        <button type="button" class="btn-close" on:click={() => submitting = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Attachments</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={handleFileSelect}
                                multiple
                                required
                            />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" on:click={() => submitting = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">Submit</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<!-- Edit Modal -->
{#if editingSubmission && submission}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form on:submit|preventDefault={handleUpdate}>
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Submission</h5>
                        <button type="button" class="btn-close" on:click={() => editingSubmission = false}></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label class="form-label">Current Files:</label>
                            {#if submission.attachments?.length}
                                <div class="d-flex flex-wrap gap-2 mb-3">
                                    {#each submission.attachments as attachment}
                                        <div class="attachment-item">
                                            <small>{attachment.fileName}</small>
                                        </div>
                                    {/each}
                                </div>
                            {/if}
                            <label class="form-label">Upload New Files:</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={handleFileSelect}
                                multiple
                                required
                            />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" on:click={() => editingSubmission = false}>
                            Cancel
                        </button>
                        <button type="submit" class="btn btn-primary">Update Submission</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
    <div class="modal-backdrop fade show"></div>
{/if}

<style>
    .attachment-preview {
        max-width: 100px;
        max-height: 100px;
        object-fit: cover;
        border-radius: 0.25rem;
    }

    .attachment-item {
        border: 1px solid #dee2e6;
        border-radius: 0.25rem;
        padding: 0.5rem;
        text-align: center;
    }

    .table-sm td, 
    .table-sm th {
        padding: 0.3rem;
    }

    .table-responsive {
        margin: 0 -1rem;
        padding: 0 1rem;
    }

    .alert-info {
        background-color: rgba(var(--bs-info-rgb), 0.1);
        border: 1px solid rgba(var(--bs-info-rgb), 0.2);
    }
</style>