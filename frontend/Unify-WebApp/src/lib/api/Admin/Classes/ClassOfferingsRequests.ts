import { api } from "$lib/api/api";
import type { UserResponse } from "$lib/api/User/UserRequests";

export interface ClassOfferingResponse{
    id: string
    name: string
    courseId: string
    startDate: string
    endDate: string
    lecturerId: string
    studentGroupId: string
    maxStudentsCount: number
}

export interface CreateClassOfferingRequest{
    name: string
    courseId: string
    startDate: string
    endDate: string
    lecturerId: string
    studentGroupId: string
    maxStudentsCount: number
}

export interface UpdateClassOfferingRequest{
    id: string
    name: string
    startDate: string
    endDate: string
    maxStudentsCount: number
}

export interface DeleteClassOfferingRequest{
    id: string
}

export const CreateClassOffering = async (data: CreateClassOfferingRequest, token: string) => {
    return await api('POST', '/class-offerings', data, token);
}

export const UpdateClassOffering = async (data: UpdateClassOfferingRequest, token: string) => {
    return await api('PUT', `/class-offerings/${data.id}`, data, token);
}

export const DeleteClassOffering = async (data: DeleteClassOfferingRequest, token: string) => {
    return await api('DELETE', `/class-offerings/${data.id}`, null, token);
}

export const AssignLecturerToClassOffering = async (classOfferingId: string, lecturerId: string, token: string) => {
    return await api('PUT', `/class-offerings/${classOfferingId}/lecturer/${lecturerId}`, null, token);
}

export const GetClassOffering = async (classOfferingId: string, token: string) => {
    return await api('GET', `/class-offerings/${classOfferingId}`, null, token);
}

export const GetClassOfferingsByLecturer = async (lecturerId: string, token: string) => {
    return await api<ClassOfferingResponse[]>('GET', `/class-offerings/lecturer/${lecturerId}`, null, token);
}

export const GetStudentsByClassOffering = async (classOfferingId: string, token: string) => {
    return await api<UserResponse[]>('GET', `/class-offerings/${classOfferingId}/students`, null, token);
}

export const GetClassOfferingsByStudent = async (studentId: string, token: string) => {
    return await api<ClassOfferingResponse[]>('GET', `/class-offerings/student/${studentId}`, null, token);
}