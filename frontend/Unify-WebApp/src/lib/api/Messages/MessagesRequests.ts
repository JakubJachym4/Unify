import { api } from "../api";

export type MessageResponse ={
    messageId: string;
    senderId: string;
    title: string;
    content: string
    createdOn: Date;
    recipientsIds: string[];
    attachments: Attachment[];
}

export type Attachment = {
    fileName: string;
    contentType: string;
    data: string
}

export type FileAttachment = File;

export interface SendMessageRequest{
    title: string;
    content: string;
    recipientsIds: string[];
    attachments: FileAttachment[];
}

export const getAllSendMessages = async (token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', '/messages/get_sent', undefined, token);
    return response;
};

export const getLastMessagesByDate = async (data: string, token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', `/messages/get_last_by_date?date=${data}`, null, token);
    return response;
};

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