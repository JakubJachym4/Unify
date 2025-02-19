<script lang="ts">
	import { GetHomeworkAssignmentById } from './../../../api/Admin/Assignments/HomeworkAssignmentRequests';
    import { GradeHomeworkSubmission } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { GradeHomeworkSubmissionRequest } from '$lib/api/Admin/Assignments/HomeworkAssignmentRequests';
    import type { HomeworkSubmission, Attachment, HomeworkAssignment } from '$lib/types/resources';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import type { ApiRequestError } from '$lib/api/apiError';
	import { CreateMark } from '$lib/api/Admin/Classes/GradeRequests';
	import { getStudentEnrollments } from '$lib/api/Admin/Classes/ClassEnrollmentsRequests';
	import { user } from '$lib/stores/user';

    export let submission: HomeworkSubmission & { student?: UserResponse };
    export let assignment: HomeworkAssignment;
    export let onClose: () => void;
    export let onGraded: () => void;

    let error = '';
    let activeTab = 'files'; // 'files' or 'grading'

    const handleGradeSubmission = async (e: Event) => {
        try {
            const form = e.target as HTMLFormElement;
            const formData = new FormData(form);
            const token = localStorage.getItem('token');
            if (!token) throw new Error('No token found');

            if(submission.id === '') {
                let enrollments = await getStudentEnrollments(submission.studentId, token);
                let enrollment = enrollments.find(enrollment => enrollment.classOfferingId === assignment.classOfferingId 
                    && enrollment.studentId === submission.studentId
                );

                if(!enrollment) throw new Error('Student not enrolled in class');

                await CreateMark({
                    title: formData.get('title') as string,
                    gradeId: enrollment.grade!.id,
                    score: Number(formData.get('score')),
                    maxScore: Number(formData.get('maxScore'))
                }, token);
            }else{
                await GradeHomeworkSubmission({
                title: formData.get('title') as string,
                assignmentId: submission.assignmentId,
                submissionId: submission.id,
                score: Number(formData.get('score')),
                maxScore: Number(formData.get('maxScore')),
                feedback: formData.get('feedback') as string
                }, token);
            }
            

            onGraded();
        } catch (err) {
            error = (err as ApiRequestError).details;
        }
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
</script>

<div class="modal fade show" style="display: block;">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">
                    Grade Submission - {submission.student?.firstName} {submission.student?.lastName}
                </h5>
                <button type="button" class="btn-close" on:click={onClose}></button>
            </div>
            <div class="modal-body">
                {#if error}
                    <div class="alert alert-danger" role="alert">{error}</div>
                {/if}

                <div class="submission-info mb-4">
                    <p class="text-muted mb-2">
                        Submitted on: {new Date(submission.submittedOn).toLocaleString()}
                    </p>
                    <p class="text-muted">
                        Student Email: {submission.student?.email}
                    </p>
                </div>

                <ul class="nav nav-tabs mb-3">
                    <li class="nav-item">
                        <a 
                            class="nav-link {activeTab === 'files' ? 'active' : ''}" 
                            href="#"
                            on:click|preventDefault={() => activeTab = 'files'}
                        >
                            Submitted Files
                        </a>
                    </li>
                    <li class="nav-item">
                        <a 
                            class="nav-link {activeTab === 'grading' ? 'active' : ''}"
                            href="#"
                            on:click|preventDefault={() => activeTab = 'grading'}
                        >
                            Grading
                        </a>
                    </li>
                </ul>

                {#if activeTab === 'files'}
                    <div class="submitted-files">
                        {#if submission.id === ''}
                            <div class="alert alert-warning">
                                <i class="bi bi-exclamation-triangle"></i>
                                This student has not submitted any files. You are creating a grade without a submission.
                            </div>
                        {:else if submission.attachments?.length}
                            <div class="row">
                                {#each submission.attachments as attachment}
                                    <div class="col-md-4 mb-3">
                                        <div class="card h-100">
                                            <div class="card-body text-center">
                                                {#if isImage(attachment)}
                                                    <img 
                                                        src={`data:${attachment.contentType};base64,${attachment.data}`}
                                                        alt={attachment.fileName}
                                                        class="img-fluid mb-2 rounded attachment-preview"
                                                        style="max-height: 200px; object-fit: contain;"
                                                    />
                                                {:else}
                                                    <i class="bi bi-{getFileIcon(attachment.fileName)} display-1"></i>
                                                {/if}
                                                <h6 class="card-title mt-2">{attachment.fileName}</h6>
                                                <a 
                                                    href={`data:${attachment.contentType};base64,${attachment.data}`}
                                                    download={attachment.fileName}
                                                    class="btn btn-outline-primary btn-sm mt-2"
                                                >
                                                    <i class="bi bi-download"></i> Download
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                {/each}
                            </div>
                        {:else}
                            <div class="alert alert-info">
                                No files were submitted
                            </div>
                        {/if}
                    </div>
                {:else}
                    <form on:submit|preventDefault={handleGradeSubmission}>
                        <div class="row">
                            <div class="mb-3">
                                <label class="form-label">Title</label>
                                <input 
                                    class="form-control"
                                    type="text"
                                    name="title"
                                    placeholder="Mark for ... submission"
                                />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Score</label>
                                <input 
                                    type="number"
                                    class="form-control"
                                    name="score"
                                    required
                                    value="0"
                                    min="0"
                                    max="100"
                                />
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="form-label">Maximum Score</label>
                                <input 
                                    type="number"
                                    class="form-control"
                                    name="maxScore"
                                    required
                                    value="100"
                                    min="0"
                                    max="100"
                                />
                            </div>
                        </div>
                        {#if submission.id !== ''}
                            <div class="mb-3">
                                <label class="form-label">Feedback</label>
                                <textarea 
                                    class="form-control"
                                    name="feedback"
                                    rows="3"
                                ></textarea>
                            </div>
                        {/if}
                        <div class="d-flex justify-content-end gap-2">
                            <button type="button" class="btn btn-secondary" on:click={onClose}>
                                Cancel
                            </button>
                            <button type="submit" class="btn btn-primary">Submit Grade</button>
                        </div>
                    </form>
                {/if}
            </div>
        </div>
    </div>
</div>
<div class="modal-backdrop fade show"></div>

<style>
    .nav-tabs {
        border-bottom: 1px solid #dee2e6;
    }

    .nav-link {
        color: #6c757d;
    }

    .nav-link.active {
        color: #0d6efd;
        border-color: #dee2e6 #dee2e6 #fff;
    }

    .card {
        transition: transform 0.2s;
    }

    .card:hover {
        transform: translateY(-5px);
        box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15);
    }
</style>