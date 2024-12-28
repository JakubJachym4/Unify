import { api } from "../api";

export type MessageResponse ={
    messageId: string;
    senderId: string;
    title: string;
    content: string
    createdOn: Date;
    recipientsIds: string[];
    attachments: FileResponse[];
}

export type FileResponse = {
    fileName: string;
    contentType: string;
    data: string
}

interface MessagesByDateRequest {
    Date: string;
}

export const getAllSendMessages = async (token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', '/messages/get_sent', undefined, token);
    return response;
};

export const getLastMessagesByDate = async (data: string, token: string): Promise<MessageResponse[]> => {
    const response = await api<MessageResponse[]>('GET', `/messages/get_last_by_date?date=${data}`, null, token);
    return response;
};