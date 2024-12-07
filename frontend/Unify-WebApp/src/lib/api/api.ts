import type { ApiRequestError } from './apiError';

export const api = async (
    method: string,
    endpoint: string,
    data: any = null,
    token: string | null = null
) => {
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
        const error: ApiRequestError = new Error('API call failed') as ApiRequestError;
        error.response = res;
        throw error;
    }

    return res.json();
};
