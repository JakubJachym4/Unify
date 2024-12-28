<script lang="ts">
    import { onMount } from 'svelte';
    import type { MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import { getUserData } from '$lib/api/User/UserRequests';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import MessageDetails from './MessageDetails.svelte';

    export let message: MessageResponse;
    let showDetails = false;
    let sender: UserResponse | null = null;
    console.log(message)

    const getDaysAgo = (date: Date) => {
        const now = new Date();
        const diffTime = Math.abs(now.getTime() - new Date(date).getTime());
        const date2 = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return date2
    };

    onMount(async () => {
        const token = localStorage.getItem('token');
        if (token) {
            sender = await getUserData(token);
        }
    });
    
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="message-card" on:click={() => showDetails = true}>
    <div class="row g-0">
        <div class="image-container">
            {#if sender?.profileImage}
                <!-- svelte-ignore a11y_img_redundant_alt -->
                <img src={sender.profileImage} 
                     class="profile-image" 
                     alt="Sender Profile Image">
            {:else}
                <!-- svelte-ignore a11y_img_redundant_alt -->
                <img src="/default-profile.png" 
                     class="profile-image" 
                     alt="Default Profile Image">
            {/if}
        </div>
        <div class="content-container">
            <div class="card-body">
                <h5 class="card-title">{message.title}</h5>
                <p class="card-text">
                    <small class="text-muted">
                        Sent by {sender?.firstName} {sender?.lastName}
                    </small>
                </p>
                <p class="card-text">
                    <small class="text-muted">
                        {getDaysAgo(message.createdOn)} days ago
                    </small>
                </p>
            </div>
        </div>
    </div>
</div>

<MessageDetails 
    {message} 
    show={showDetails} 
    onClose={() => showDetails = false}
/>

<style>
    .message-card {
        border: 3px solid transparent;
        border-radius: var(--bs-border-radius);
        transition: all 0.2s ease-in-out;
        margin-bottom: 0.5rem;
        background: var(--bs-card-bg);
        min-width: 350px;
        cursor: pointer;
    }

    .message-card:hover {
        border-color: var(--bs-primary);
        transform: translateY(-2px);
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    }

    .image-container {
        width: 100px;
        height: 100px;
        padding: 0;
        overflow: hidden;
    }

    .profile-image {
        width: 100%;
        height: 100%;
        object-fit: cover;
    }

    .content-container {
        flex: 1;
        padding-left: 0.5rem;
        padding-top: 0.5rem;
    }

    .row {
        display: flex;
        margin: 0;
    }
</style>