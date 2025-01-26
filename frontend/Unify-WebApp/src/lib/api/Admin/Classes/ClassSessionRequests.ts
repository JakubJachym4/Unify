import { api } from "$lib/api/api";
import { ClassType, type ClassSession } from "$lib/types/resources";

export interface CreateClassSessionRequest{
    classOfferingId: string,
    title: string,
    scheduledDate: string,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export interface UpdateClassSessionRequest{
    id: string,
    title: string,
    scheduledDate: string,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export interface DeleteClassSessionRequest{
    id: string,
}

export interface CreateIntervalClassSessionsRequest{
    classOfferingId: string,
    title: string,
    startDate: string,
    endDate: string,
    weekInterval: number,
    duration: string,
    lecturerId: string,
    locationId: string,
}

export const CreateClassSession= async (data: CreateClassSessionRequest, token: string) => {
    return await api('POST', '/class-offering-sessions', data, token);
}

export const UpdateClassSession = async (data: UpdateClassSessionRequest, token: string) => {
    return await api('PUT', `/class-offering-sessions/${data.id}`, data, token);
}

export const DeleteClassSession = async (data: DeleteClassSessionRequest, token: string) => {
    return await api('DELETE', `/class-offering-sessions/${data.id}`, null, token);
}

export const GetClassSession = async (classSessionId: string, token: string) : Promise<ClassSession> => {
    let classSessions = await api<ClassSession>('GET', `/class-offering-sessions/${classSessionId}`, null, token);
    classSessions.classType = ClassType.class;
    return classSessions;
}

export const GetClassSessions = async (token: string) : Promise<ClassSession[]> => {
    let classSessions = await api<ClassSession[]>('GET', '/class-offering-sessions', null, token);
    classSessions.forEach(resource => {
        resource.classType = ClassType.class;
    });

    return classSessions;
}

export const GetClassSessionByClassOffering = async (classOfferingId: string, token: string) : Promise<ClassSession[]> => {
    let classSessions = await api<ClassSession[]>('GET', `/class-offering-sessions/course/${classOfferingId}`, null, token);
    classSessions.forEach(resource => {
        resource.classType = ClassType.class;
    });

    return classSessions;
}

export const CreateIntervalClassSessions = async (data: CreateIntervalClassSessionsRequest, token: string) => {
    return await api('POST', '/class-offering-sessions/create-interval', data, token);
}