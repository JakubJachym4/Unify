// src/lib/api/User/UserRequests.ts

import { api } from "$lib/api/api";
import type { ApiRequestError } from "$lib/api/apiError";

// Add these interfaces
interface AddRoleRequest {
    userId: string;
    role: string;
}

interface DeleteRoleRequest {
    userId: string;
    role: string;
}

// Add these functions
export const addUserRole = async (data: AddRoleRequest, token: string) => {
    try {
        await api('POST', '/admin/users/add-role', data, token);
        return true;
    } catch (error) {
        const apiError = error as ApiRequestError;
        throw new Error(apiError.details || 'Failed to add role');
    }
};

export const deleteUserRole = async (data: DeleteRoleRequest, token: string) => {
    try {
        await api('POST', '/admin/users/delete-role', data, token);
        return true;
    } catch (error) {
        const apiError = error as ApiRequestError;
        throw new Error(apiError.details || 'Failed to delete role');
    }
};