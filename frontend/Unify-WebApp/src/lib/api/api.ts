import type { ApiRequestError } from './apiError';

export const api: {
    (method: string, endpoint: string, data?: any, token?: string | null) : Promise<any>;
    <T>(method: string, endpoint: string, data?: any, token?: string | null): Promise<T>;
} = async <T>(
    method: string,
    endpoint: string,
    data: any = null,
    token: string | null = null
): Promise<T> => {
    const headers: Record<string, string> = {
        'Content-Type': 'application/json',
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }
    endpoint = "https://localhost:5001/api" + endpoint;
    const res = await fetch(endpoint, {
        method,
        headers,
        body: data ? JSON.stringify(data) : null,
    });

    if (!res.ok) {
        const errorData = await res.json();
        const error: ApiRequestError = new Error('API call failed') as ApiRequestError;
        error.response = res;
        error.code = errorData.code;
        error.details = errorData.details;
        throw error;
    }

    const responseData: T = await res.json();
    return responseData;
};