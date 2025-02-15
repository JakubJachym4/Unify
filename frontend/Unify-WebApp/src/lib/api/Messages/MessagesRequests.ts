import type { Attachment } from "$lib/types/resources";
import { api } from "../api";

export const enum SeverityLevel {
    Critical = 0,
    Important = 1,
    Information = 2,
    Notification = 3
}

export const getSeverityColor = (severity: string | null): string => {
    if (!severity) return '';
    switch (severity) {
        case 'Critical': return 'var(--bs-danger)';
        case 'Important': return 'var(--bs-warning)';
        case 'Information': return 'var(--bs-info)';
        case 'Notification': return 'var(--bs-primary)';
        default: return '';
    }
}

export type MessageResponse ={
    messageId: string;
    senderId: string;
    title: string;
    content: string
    createdOn: Date;
    recipientsIds: string[];
    attachments: Attachment[];
    respondingToId: string | null;
    forwardedFromId: string | null;
    expirationDate: Date | null;
    informationSeverityLevel: string | null;
}

export type FileAttachment = File;

export interface SendMessageRequest{
    title: string;
    content: string;
    recipientsIds: string[];
    attachments: FileAttachment[];
}

export interface SendNotificationRequest{
    title: string;
    content: string;
    recipientsIds: string[];
    attachments: FileAttachment[];
    severity: string;
    expirationDate: Date;
}



export interface ReplyToMessageRequest{
    title: string;
    content: string;
    recipientsIds: string[];
    attachments: FileAttachment[];
    respondingToId: string;
}

export interface ForwardMessageRequest{
    originalMessageId: string;
    newRecipientsIds: string[];
}

export const getAllSendMessages = async (token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', '/messages/get_sent', undefined, token);
    return response;
};

export const getLastMessagesByDate = async (data: string, token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', `/messages/get_last_by_date?date=${data}`, null, token);
    return response;
};

export const getLastMessagesByNumber = async (data: number, token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', `/messages/get_last_by_number/${data}`, null, token);
    return response;
}

export const getNonExpiredNotifications = async (token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', '/notifications', null, token);
    return response;
}

export const sendMessage = async (data: SendMessageRequest, token: string): Promise<string> => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('content', data.content);
    data.recipientsIds.forEach(id => {
        formData.append('recipientsIds', id);
    });
    data.attachments.forEach(file => {
        formData.append('attachments', file);
    });

    const response = await api<string>('POST', '/messages/send', formData, token, true);
    return response;
};

export const sendNotification = async (data: SendNotificationRequest, token: string): Promise<string> => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('content', data.content);
    data.recipientsIds.forEach(id => {
        formData.append('recipientsIds', id);
    });
    data.attachments.forEach(file => {
        formData.append('attachments', file);
    });

    formData.append('severity', data.severity);
    formData.append('expirationDate', data.expirationDate.toString());

    const response = await api<string>('POST', '/notifications/send', formData, token, true);
    return response;
}

export const replyToMessage = async (data: ReplyToMessageRequest, token: string): Promise<string> => {
    const formData = new FormData();
    formData.append('title', data.title);
    formData.append('content', data.content);
    data.recipientsIds.forEach(id => {
        formData.append('recipientsIds', id);
    });
    formData.append('respondingToId', data.respondingToId);
    data.attachments.forEach(file => {
        formData.append('attachments', file);
    });

    const response = await api<string>('POST', '/messages/reply', formData, token, true);
    return response;
};

export const forwardMessage = async (data: ForwardMessageRequest, token: string): Promise<string> => {
    const formData = new FormData();
    formData.append('originalMessageId', data.originalMessageId);
    data.newRecipientsIds.forEach(id => {
        formData.append('newRecipientsIds', id);
    });

    const response = await api<string>('POST', '/messages/forward', formData, token, true);
    return response;
}