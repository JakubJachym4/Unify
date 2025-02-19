<script lang="ts">
	import { facultiesStore } from '$lib/stores/university';
	import { GetStudentsByClassOffering } from '$lib/api/Admin/Classes/ClassOfferingsRequests';
    import { onMount } from 'svelte';
    import { 
        GetHomeworkAssignmentById,
        UpdateHomeworkAssignment,
        GradeHomeworkSubmission,
        SetAssignmentLock
    } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import { 
        GetHomeworkSubmissionsByAssignment 
    } from '$lib/api/Admin/Assignments/HomeworkSubmissionsRequests';
    import type { 
        UpdateHomeworkAssignmentRequest,
        GradeHomeworkSubmissionRequest 
    } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
    import type { HomeworkAssignment, HomeworkSubmission, Attachment } from '$lib/types/resources';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import { convertFilesToAttachments, convertAttachmentsToFiles } from '$lib/types/resources';
    import SubmissionGrading from './SubmissionGrading.svelte';

    export let assignmentId: string;
    export let classOfferingId: string;
    export let onBack: () => void;

    let assignment: HomeworkAssignment | null = null;
    let submissions: (HomeworkSubmission & { student?: UserResponse })[] = [];
    let students: UserResponse[] = [];
    let error = '';
    let loading = true;
    let editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: null as string | null,
                            dueDate: '',
                            attachments: null as File[] | null

                        }
    let editingAssignmentState = false;
    let gradingSubmission: HomeworkSubmission | null = null;

    const loadData = async () => {
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            const [assignmentData, submissionsData, studentsData] = await Promise.all([
                GetHomeworkAssignmentById(assignmentId, token),
                GetHomeworkSubmissionsByAssignment(assignmentId, token),
                GetStudentsByClassOffering(classOfferingId, token)
            ]);

            assignment = assignmentData;
            students = studentsData;
            
            submissions = submissionsData.map(submission => ({
                ...submission,
                student: students.find(s => s.id === submission.studentId)
            }));

            loading = false;
        } catch (err) {
            error = (err as ApiRequestError).details;
            loading = false;
        }
    };

    const handleUpdate = async (id: string) => {
        if (!assignment) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            if(!editingAssignment) return;

            // Handle attachments properly
            const updateRequest: UpdateHomeworkAssignmentRequest = {
                ...editingAssignment,
                // If new attachments were uploaded, use them; otherwise keep existing ones
                attachments: editingAssignment.attachments 
                    ? editingAssignment.attachments 
                    : convertAttachmentsToFiles(assignment.attachments) || []
            };

            await UpdateHomeworkAssignment(updateRequest, token);
            editingAssignmentState = false;
            editingAssignment = {
                id: '',
                title: '',
                description: '',
                criteria: null as string | null,
                dueDate: '',
                attachments: null as File[] | null
            };
            await loadData();
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
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    const handleLockToggle = async () => {
        if (!assignment) return;
        try {
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');
            
            await SetAssignmentLock(assignment.id, !assignment.locked, token);
            await loadData();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
    };

    // Helper functions
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

    const handleFileSelect = (event: Event, isEdit = false) => {
        const target = event.target as HTMLInputElement;
        const files = Array.from(target.files || []);
        if (editingAssignment && files.length > 0) {
            editingAssignment = {
                ...editingAssignment,
                attachments: files
            };
        }
    };

    onMount(loadData);
</script>

<div class="container mt-4">
    <div class="d-flex justify-content-between align-items-center mb-4">
        <h2>Assignment Submissions</h2>
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
        <!-- Assignment Details Card -->
        <div class="card mb-4">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h5 class="mb-0">Assignment Details</h5>
                <div class="btn-group">
                    <button 
                        class="btn btn-sm {assignment.locked ? 'btn-warning' : 'btn-outline-warning'}"
                        on:click={handleLockToggle}
                        title={assignment.locked ? 'Unlock Assignment' : 'Lock Assignment'}
                    >
                        <i class="bi bi-{assignment.locked ? 'lock-fill' : 'unlock-fill'}"></i>
                        {assignment.locked ? 'Locked' : 'Unlocked'}
                    </button>
                    <button 
                        class="btn btn-sm btn-outline-primary ms-2"
                        on:click={() => {
                            editingAssignment = {
                                ...assignment!, 
                                attachments: convertAttachmentsToFiles(assignment!.attachments) || []
                            }; 
                            editingAssignmentState = true
                        }}
                        disabled={assignment.locked}
                    >
                        Edit Assignment
                    </button>
                </div>
            </div>
            <div class="card-body">
                <h5 class="card-title">{assignment.title}</h5>
                <div class="d-flex align-items-center mb-2">
                    {#if assignment.locked}
                        <span class="badge bg-warning me-2">
                            <i class="bi bi-lock-fill"></i> Locked
                        </span>
                    {/if}
                </div>
                <p class="card-text">{assignment.description}</p>
                <p class="card-text">Grading Criteria: {assignment.criteria ?? "Not Defined"}</p>
                <p class="card-text">
                    <small class="text-muted">Due: {new Date(assignment.dueDate).toLocaleString()}</small>
                </p>
                {#if assignment.attachments?.length}
                    <div class="mt-3">
                        <h6>Attachments:</h6>
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

        <!-- Submissions Section -->
        <h3 class="mb-3">Submissions</h3>
        <div class="row">
            <!-- Submitted -->
            <div class="col-md-6 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="mb-0">Submitted ({submissions.length})</h5>
                    </div>
                    <div class="card-body">
                        <div class="list-group">
                            {#each submissions as submission}
                                <div class="list-group-item">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h6 class="mb-1">
                                                {submission.student?.firstName} {submission.student?.lastName}
                                            </h6>
                                            <small class="text-muted">
                                                Submitted: {new Date(submission.submittedOn).toLocaleString()}
                                            </small>
                                        </div>
                                        {#if !submission.mark}
                                                <button 
                                                class="btn btn-sm btn-outline-primary"
                                                on:click={() => gradingSubmission = submission}
                                            >
                                                Grade
                                            </button>
                                        {:else}
                                            <div>
                                                <span class="badge bg-success">
                                                    <i class="bi bi-check2"></i> Graded
                                                </span>
                                            </div>
                                        {/if}
                                        
                                    </div>
                                </div>
                            {/each}
                        </div>
                    </div>
                </div>
            </div>

            <!-- Not Submitted -->
            <div class="col-md-6 mb-4">
                <div class="card h-100">
                    <div class="card-header">
                        <h5 class="mb-0">Not Submitted ({students.length - submissions.length})</h5>
                    </div>
                    <div class="card-body">
                        <div class="list-group">
                            {#each students.filter(student => 
                                !submissions.find(s => s.studentId === student.id)
                            ) as student}
                                <div class="list-group-item">
                                    <div class="d-flex justify-content-between align-items-center">
                                        <div>
                                            <h6 class="mb-1">{student.firstName} {student.lastName}</h6>
                                            <small class="text-muted">{student.email}</small>
                                        </div>
                                        <button 
                                            class="btn btn-sm btn-outline-warning"
                                            disabled={!assignment.locked}
                                            title="Grading is disabled when assignment is not locked"
                                            on:click={() => {
                                                gradingSubmission = {
                                                    id: '',
                                                    assignmentId: assignmentId,
                                                    studentId: student.id,
                                                    mark: null,
                                                    feedback: '',
                                                    submittedOn: new Date().toISOString(),
                                                    attachments: []
                                                };
                                            }}
                                        >
                                            <i class="bi bi-exclamation-triangle"></i> Grade Without Submission
                                        </button>
                                    </div>
                                </div>
                            {/each}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    {/if}
</div>

<!-- Edit Assignment Modal -->
{#if editingAssignment && assignment && editingAssignmentState}
    <div class="modal fade show" style="display: block;">
        <div class="modal-dialog">
            <div class="modal-content">
                <form on:submit|preventDefault={() => handleUpdate(assignmentId)}>
                    <div class="modal-header">
                        <h5 class="modal-title">Edit Assignment</h5>
                        <button type="button" class="btn-close" on:click={() => {editingAssignment = {
                            id: '',
                            title: '',
                            description: '',
                            criteria: null as string | null,
                            dueDate: '',
                            attachments: null as File[] | null

                        }; editingAssignmentState = false}}></button>
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
                            <label class="form-label">Current Attachments</label>
                            {#if assignment.attachments?.length}
                                <div class="d-flex flex-wrap gap-2 mb-3">
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
                                        </div>
                                    {/each}
                                </div>
                            {/if}
                            <label class="form-label">Upload New Attachments</label>
                            <input 
                                type="file"
                                class="form-control"
                                on:change={(e) => handleFileSelect(e, true)}
                                multiple
                            />
                            <small class="form-text text-muted">
                                Leave empty to keep current attachments. Uploading new files will replace all existing attachments.
                            </small>
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

                        } ; editingAssignmentState = false}}>
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

<!-- Grade Submission Modal -->
{#if gradingSubmission}
    <SubmissionGrading
        submission={gradingSubmission}
        assignment={assignment!}
        onClose={() => gradingSubmission = null}
        onGraded={async () => {
            gradingSubmission = null;
            await loadData();
        }}
    />
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
</style>