<script lang="ts">
	import { forwardMessage } from './../../api/Messages/MessagesRequests.ts';
    import { onMount } from 'svelte';
    import { getSeverityColor, type MessageResponse, type SeverityLevel } from '$lib/api/Messages/MessagesRequests';
    import type { UserResponse } from '$lib/api/User/UserRequests';
    import MessageDetails from './MessageDetails.svelte';
	import { get } from 'svelte/store';
	import { globalUsers } from '$lib/stores/globalUsers';
	import { messagesStore } from '$lib/stores/messages';

    export let message: MessageResponse;
    let showDetails = false;
    let sender: UserResponse | null = null;
    let forwardedFrom: UserResponse | null = null;

    $: borderColor = message.informationSeverityLevel ?
        getSeverityColor(message.informationSeverityLevel) : 'transparent'

    $: isNotification = !!message.informationSeverityLevel;
    $: severityColor = message.informationSeverityLevel ? 
        getSeverityColor(message.informationSeverityLevel) : 
        'transparent';

    const getDaysAgo = (date: Date) => {
        const now = new Date();
        const diffTime = Math.abs(now.getTime() - new Date(date).getTime());
        const date2 = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        return date2
    };

    onMount(async () => {
        sender = get(globalUsers).find(u => u.id === message.senderId) || null;
        if (message.forwardedFromId) {
            const allMessages = get(messagesStore).messages;
            const forwardedMessage = allMessages.find(m => m.messageId === message.forwardedFromId);
            if (forwardedMessage) {
                forwardedFrom = get(globalUsers).find(u => u.id === forwardedMessage.senderId) || null;
            }
        }
    });

    const handleClick = (e: MouseEvent) => {
        e.stopPropagation();
        showDetails = true;
    }

    
</script>

<!-- svelte-ignore a11y_click_events_have_key_events -->
<!-- svelte-ignore a11y_no_static_element_interactions -->
<div class="message-card" on:click={handleClick}
    style="border-color: {severityColor}">
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
                <div class="d-flex">
                    {#if isNotification}
                    <h5 class="card-title" style="color: {severityColor}">
                        {message.informationSeverityLevel}:&nbsp
                    </h5>
                    {/if}
                    <h5 class="card-title">{message.title}</h5>
                </div>
                <p class="card-text mb-0">
                    <small class="text-muted">
                        {#if message.forwardedFromId && forwardedFrom}
                            Forwarded from {forwardedFrom.firstName} {forwardedFrom.lastName}
                        {:else}  
                            Sent by {sender?.firstName} {sender?.lastName}
                        {/if}
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
        hideReplyButton={message.forwardedFromId != null}
    />


<style>
    .message-card {
        border: 4px solid transparent;
        border-radius: var(--bs-border-radius);
        transition: all 0.2s ease-in-out;
        margin-bottom: 0.5rem;
        background: var(--bs-card-bg);
        min-width: 350px;
        cursor: pointer;
    }

    .message-card:hover {
        transform: translateY(-4px);
        box-shadow: 0 6px 8px rgba(0,0,0,0.1);
        background-color: #99a5b4;
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
        /* padding-top: 0.5rem; */
    }

    .row {
        display: flex;
        margin: 0;
    }

    .severity-label {
        font-weight: 600;
        font-size: 0.9rem;
    }
</style>