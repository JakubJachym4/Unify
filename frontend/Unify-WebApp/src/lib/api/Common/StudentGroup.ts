import { api } from "../api";
import { type StudentGroup } from "$lib/types/universityClasses";

export interface CreateStudentGroupForSpecializationRequest {
    name: string
    specializationId: string
    studyYear: number
    semester: number
    term: string
    combinedSize: number
    maxGroupSize: number
}
export interface CreateStudentGroupRequest {
    name: string
    specializationId: string
    studyYear: number
    semester: number
    term: string
    maxGroupSize: number
}

export interface UpdateStudentGroupRequest {
    id: string
    name: string
    specializationId: string
    studyYear: number
    semester: number
    term: string
    maxGroupSize: number
}


export interface MoveUserToGroupRequest {
    userId: string
    groupId: string | null
}



export const getStudentGroups = async (token: string): Promise<StudentGroup[]> => {
  return await api<StudentGroup[]>('GET', "/student-groups", null, token);
};

export const getStudentGroupByStudentId = async (studentId: string, token: string): Promise<StudentGroup> => {
    return await api<StudentGroup>('GET', `/student-groups/user/${studentId}`, null, token);
}

export const getStudentGroupsBySpecializationId = async (specializationId: string, token: string): Promise<StudentGroup[]> => {
    return await api<StudentGroup[]>('GET', `/student-groups/specialization/${specializationId}`, null, token);
}

export const createStudentGroupMultiple = async (request: CreateStudentGroupForSpecializationRequest, token: string) => {
    await api('POST', "/student-groups/create-multiple-for-specialization", request, token);
}

export const createStudentGroup = async (request: CreateStudentGroupRequest, token: string) => {
    await api('POST', "/student-groups/create", request, token);
}

export const moveUserToGroup = async (request: MoveUserToGroupRequest, token: string) => {
    await api('PUT', `/student-groups/user/${request.userId}`, request, token);
}

export const deleteStudentGroup = async (studentGroupId: string, token: string) => {
    await api('DELETE', `/student-groups/${studentGroupId}`, null, token);
}

export const updateStudentGroup = async (request: UpdateStudentGroupRequest, token: string) => {
    await api('PUT', `/student-groups/${request.id}`, request, token);
}

export const autoAssignSpecializationStudentsToGroups = async (id: string, token: string) => {
    await api('POST', `/student-groups/specialization/${id}/auto-assign`, null, token);
}