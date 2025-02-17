import { api } from '$lib/api/api';


export interface EnrollmentRequest{
    studentId: string,
    classOfferingId: string,
}

export const getStudentEnrollments = async (id: string, token: string): Promise<StudentEnrollment[]> => {
  return await api<StudentEnrollment[]>('GET', `/class-enrollments/student/${id}`, null, token);
}

export const getEnrollmentsByClassOffering = async (id: string, token: string): Promise<StudentEnrollment[]> => {
    return await api<StudentEnrollment[]>('GET', `/class-enrollments/class-offering/${id}`, null, token);
}

export const getEnrollmentById = async (id: string, token: string): Promise<StudentEnrollment> => {
    return await api<StudentEnrollment>('GET', `/class-enrollments/${id}`, null, token);
}

export const enrollStudent = async (data: EnrollmentRequest, token: string) => {
    return await api('POST', '/class-enrollments/enroll', data, token);
}

export const cancelStudentEnrollment = async (data: EnrollmentRequest, token: string) => {
    return await api('POST', '/class-enrollments/cancel', data, token);
}

export const getGradeById = async (id: string, token: string): Promise<StudentEnrollment> => {
    return await api<StudentEnrollment>('GET', `/grades/${id}`, null, token);
}