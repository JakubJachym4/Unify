import { api } from "$lib/api/api";
import type { HomeworkSubmission } from "$lib/types/resources";

export interface CreateHomeworkSubmissionRequest{
    assignmentId: string,
    attachments: File[] | null,
}

export interface UpdateHomeworkAssignmentRequest{
    id: string,
    attachments: File[] | null,
}

export interface DeleteHomeworkAssignmentRequest{
    id: string,
}

export const CreateHomeworkSubmission = async (data: CreateHomeworkSubmissionRequest, token: string) => {
    const fromData = new FormData();
    fromData.append('HomeworkAssignmentId', data.assignmentId);
    if(data.attachments){
        data.attachments.forEach(file => {
            fromData.append('attachments', file);
        });
    }
    return await api('POST', `/assignments/${data.assignmentId}/submit`, fromData, token, true);
}

export const UpdateHomeworkSubmission = async (data: UpdateHomeworkAssignmentRequest, token: string) => {
    const fromData = new FormData();
    fromData.append('id', data.id);
    if(data.attachments){
        data.attachments.forEach(file => {
            fromData.append('attachments', file);
        });
    }
    return await api('PUT', `/assignments/submissions/${data.id}`, fromData, token, true);
}

export const DeleteHomeworkSubmission = async (data: DeleteHomeworkAssignmentRequest, token: string) => {
    return await api('DELETE', `/assignments/submissions/${data.id}`, null, token);
}

export const GetHomeworkSubmission = async (submissionId: string, token: string) : Promise<HomeworkSubmission> => {
    let submission = await api<HomeworkSubmission>('GET', `/assignments/submissions/${submissionId}`, null, token);
    return submission;
}

export const GetHomeworkSubmissionsByAssignment = async (assignmentId: string, token: string) : Promise<HomeworkSubmission[]> => {
    let submissions = await api<HomeworkSubmission[]>('GET', `/assignments/${assignmentId}/submissions`, null, token);
    return submissions;
}

export const GetHomeworkSubmissionsByStudent = async (studentId: string, token: string) : Promise<HomeworkSubmission[]> => {
    let submissions = await api<HomeworkSubmission[]>('GET', `/assignments/student/${studentId}/submissions`, null, token);
    return submissions;
}