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