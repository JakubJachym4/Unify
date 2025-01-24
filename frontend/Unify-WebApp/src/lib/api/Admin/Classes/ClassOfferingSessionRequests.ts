import { api } from "$lib/api/api";
import type { ClassOfferingSession } from "$lib/types/universityClasses";

export interface CreateClassOfferingSessionRequest{
    classOfferingId: string
    title: string
    scheduledDate: string
    duration: number
    lecturerId: string
    locationId: string
}

export interface UpdateClassOfferingSessionRequest{
    id: string
    title: string
    scheduledDate: string
    duration: number
    lecturerId: string
    locationId: string
}

export interface DeleteClassOfferingSessionRequest{
    id: string
}

export const CreateClassOfferingSession = async (data: CreateClassOfferingSessionRequest, token: string) => {
    return await api('POST', '/class-offering-sessions', data, token);
}

export const UpdateClassOfferingSession = async (data: UpdateClassOfferingSessionRequest, token: string) => {
    return await api('PUT', `/class-offering-sessions/${data.id}`, data, token);
}

export const DeleteClassOfferingSession = async (data: DeleteClassOfferingSessionRequest, token: string) => {
    return await api('DELETE', `/class-offering-sessions/${data.id}`, null, token);
}

export const GetClassOfferingSession = async (classOfferingSessionId: string, token: string) => {
    return await api<ClassOfferingSession>('GET', `/class-offering-sessions/${classOfferingSessionId}`, null, token);
}

export const GetClassOfferingSessions = async (token: string) => {
    return await api<ClassOfferingSession[]>('GET', '/class-offering-sessions', null, token);
}

export const GetClassOfferingSessionByClassOffering = async (classOfferingId: string, token: string) => {
    return await api<ClassOfferingSession[]>('GET', `/class-offering-sessions/class-offering/${classOfferingId}`, null, token);
}
export const GetClassOfferingSessionByLecturer = async (lecturerId: string, token: string) => {
    return await api<ClassOfferingSession[]>('GET', `/class-offering-sessions/lecturer/${lecturerId}`, null, token);
}

export const GetClassOfferingSessionByStudent = async (studentId: string, token: string) => {
    return await api<ClassOfferingSession[]>('GET', `/class-offering-sessions/student/${studentId}`, null, token);
}