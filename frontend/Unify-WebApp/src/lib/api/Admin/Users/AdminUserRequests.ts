// src/lib/api/User/UserRequests.ts

import { api } from "$lib/api/api";
import type { ApiRequestError } from "$lib/api/apiError";

// Add these interfaces
interface AddRoleRequest {
    userId: string;
    role: string;
}

interface RemoveRoleRequest {
    userId: string;
    role: string;
}

// Add these functions
export const addUserRole = async (data: AddRoleRequest, token: string) => {
        await api('POST', '/admin/users/add-role', data, token);
        return true;
};

export const removeUserRole = async (data: RemoveRoleRequest, token: string) => {
        await api('POST', '/admin/users/remove-role', data, token);
        return true;

};