import { api } from "$lib/api/api";
import { ClassType, type ClassResource, type ClassSession } from "$lib/types/resources";

export interface CreateLectureRequest{
    courseId: string,
    title: string,
    scheduledDate: string,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export interface UpdateLectureRequest{
    id: string,
    title: string,
    scheduledDate: string,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export interface DeleteLectureRequest{
    id: string,
}

export interface CreateIntervalLecturesRequest{
    courseId: string,
    title: string,
    startDate: string,
    endDate: string,
    weekInterval: number,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export const CreateLecture = async (data: CreateLectureRequest, token: string) => {
    return await api('POST', '/lectures', data, token);
}

export const UpdateLecture = async (data: UpdateLectureRequest, token: string) => {
    return await api('PUT', `/lectures/${data.id}`, data, token);
}

export const DeleteLecture = async (data: DeleteLectureRequest, token: string) => {
    return await api('DELETE', `/lectures/${data.id}`, null, token);
}

export const GetLecture = async (lectureId: string, token: string) : Promise<ClassSession> => {
    let lecture = await api<ClassSession>('GET', `/lectures/${lectureId}`, null, token);
    lecture.classType = ClassType.lecture;
    return lecture;
}

export const GetLectures = async (token: string) : Promise<ClassSession[]> => {
    let lectures = await api<ClassSession[]>('GET', '/lectures', null, token);
    lectures.forEach(resource => {
        resource.classType = ClassType.lecture;
    });

    return lectures;
}

export const GetLecturesByCourse = async (courseId: string, token: string) : Promise<ClassSession[]> => {
    let lectures = await api<ClassSession[]>('GET', `/lectures/course/${courseId}`, null, token);
    lectures.forEach(resource => {
        resource.classType = ClassType.lecture;
    });

    return lectures;
}

export const CreateIntervalLectures = async (data: CreateIntervalLecturesRequest, token: string) => {
    return await api('POST', '/lectures/create-interval', data, token);
}