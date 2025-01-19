import { api } from "$lib/api/api";
import { specializationsStore } from "$lib/stores/university";
import type { Course } from "$lib/types/universityClasses";
import type { ClassOfferingResponse } from "./ClassOfferingsRequests";

export interface CreateCourseRequest {
    name: string
    description: string
    specializationId: string
}

export interface UpdateCourseRequest {
    id: string
    name: string
    description: string
}

export interface DeleteCourseRequest { id: string}

export interface CourseResponse{
    id: string
    name: string
    description: string
    specializationId: string
    lecturerId: string
    classOfferings: ClassOfferingResponse[]
}

export const CreateCourse = async (data: CreateCourseRequest, token: string) => {
    return await api('POST', '/courses', data, token);
}
export const UpdateCourse = async (data: UpdateCourseRequest, token: string) => {
    return await api('PUT', `/courses/${data.id}`, data, token);
}

export const DeleteCourse = async (data: DeleteCourseRequest, token: string) => {
    return await api('DELETE', `/courses/${data.id}`, null, token);
}

export const GetCourses = async (token: string) => {
    return await api('GET', '/courses', null, token);
}

export const GetCourseBySpecialization = async (specializationId: string, token: string) => {
    return await api<CourseResponse[]>('GET', `/courses/specialization/${specializationId}`, null, token);
}

export const AssignLecturer = async (courseId: string, lecturerId: string, token: string) => {
    return await api('PUT', `/courses/${courseId}/lecturer/${lecturerId}`, null, token);
}

export const GetCoursesByLecturer = async (lecturerId: string, token: string) => {
    return await api<CourseResponse[]>('GET', `/courses/lecturer/${lecturerId}`, null, token);
}

export const GetCourseById = async (courseId: string, token: string) => {
    return await api<CourseResponse>('GET', `/courses/${courseId}`, null, token);
}