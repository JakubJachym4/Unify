import type { UserResponse } from "$lib/api/User/UserRequests"
import type { Specialization } from "./university"

export interface StudentGroup{
    id: string
    name: string
    specializationId: string
    studyYear: number
    semester: number
    term: string
    maxGroupSize: number
    members: string[]
    classOfferingResponse: object | null
}
export type ManagementView = 'specializations' | 'studentGroups';

export interface Course{
    id: string
    name: string
    description: string
    specialization: Specialization
    lecturer: UserResponse | null
    classOfferings: ClassOffering[]
}

export interface ClassOffering{
    id: string
    name: string
    courseId: string
    startDate: string
    endDate: string
    lecturerId: string
    studentGroupId: string
    maxStudentsCount: number
}