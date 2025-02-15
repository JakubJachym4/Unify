import { api } from "$lib/api/api";
import type { HomeworkAssignment } from "$lib/types/resources";

export interface CreateHomeworkAssignmentRequest{
    classOfferingId: string,
    title: string,
    description: string,
    dueDate: string,
    attachments: File[] | null,
}

export interface UpdateHomeworkAssignmentRequest{
    id: string,
    title: string,
    description: string,
    dueDate: string,
    attachments: File[] | null,
}

export interface DeleteHomeworkAssignmentRequest{
    id: string,
}

export interface GradeHomeworkSubmissionRequest{
    assignmentId: string,
    submissionId: string,
    score: number,
    maxScore: number,
    criteria: string,
    feedback: string,
}

export const CreateHomeworkAssignment = async (data: CreateHomeworkAssignmentRequest, token: string) => {
    const fromData = new FormData();
    fromData.append('classOfferingId', data.classOfferingId);
    fromData.append('title', data.title);
    fromData.append('description', data.description);
    fromData.append('dueDate', data.dueDate);
    if(data.attachments){
        data.attachments.forEach(file => {
            fromData.append('attachments', file);
        });
    }
    return await api('POST', `/assignments/class-offering/${data.classOfferingId}`, fromData, token, true);
}

export const UpdateHomeworkAssignment = async (data: UpdateHomeworkAssignmentRequest, token: string) => {
    const fromData = new FormData();
    fromData.append('id', data.id);
    fromData.append('title', data.title);
    fromData.append('description', data.description);
    fromData.append('dueDate', data.dueDate);
    if(data.attachments){
        data.attachments.forEach(file => {
            fromData.append('attachments', file);
        });
    }
    return await api('PUT', `/assignments/${data.id}`, fromData, token, true);
}

export const DeleteHomeworkAssignment = async (data: DeleteHomeworkAssignmentRequest, token: string) => {
    return await api('DELETE', `/assignments/${data.id}`, null, token);
}

export const GradeHomeworkSubmission = async (data: GradeHomeworkSubmissionRequest, token: string) => {
    return await api('POST', `/assignments/${data.assignmentId}/submissions/${data.submissionId}/grade`, data, token);
}

export const GetHomeworkAssignmentById = async (id: string, token: string) => {
    return await api<HomeworkAssignment>('GET', `/assignments/${id}`, null, token);
}

export const GetHomeworkAssignmentsByClassOfferingId = async (classOfferingId: string, token: string) => {
    return await api<HomeworkAssignment[]>('GET', `/assignments/class-offering/${classOfferingId}`, null, token);
}

export const GetHomeworkAssignmentsByStudentId = async (studentId: string, token: string) => {
    return await api<HomeworkAssignment[]>('GET', `/assignments/student/${studentId}`, null, token);
}
