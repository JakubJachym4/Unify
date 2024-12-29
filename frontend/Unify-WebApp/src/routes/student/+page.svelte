<script lang="ts">
    import { onMount } from 'svelte';
    import { getLastMessagesByDate, type MessageResponse } from '$lib/api/Messages/MessagesRequests';
    import MessagesList from '$lib/components/messages/MessagesList.svelte';
    import NewMessage from '$lib/components/messages/NewMessage.svelte';
    import MessagesContainer from '$lib/components/messages/MessagesContainer.svelte';

    let messages = [];
    let error = '';
    let loading = true;
    let showNewMessage = false;
    let showSuccessAlert = false;

    const handleMessageSent = () => {
        showSuccessAlert = true;
        setTimeout(() => {
            showSuccessAlert = false;
        }, 3000); // Hide after 3 seconds
    };

</script>

{#if showSuccessAlert}
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        Message sent successfully!
        <button type="button" class="btn-close" on:click={() => showSuccessAlert = false}></button>
    </div>
{/if}

<div class="container-fluid mt-4">
    <div class="row h-100">
        <div class="col-md-8">
            <h1>Student Dashboard</h1>
            <!-- Add other student dashboard content here -->
        </div>
        <div class="col-md-4">
            <MessagesContainer fixed={false} />
        </div>
    </div>
</div>

<!-- Add NewMessage component -->
<NewMessage 
    show={showNewMessage}
    onClose={() => showNewMessage = false}
    on:messageSent={handleMessageSent}
/>

<style>
    .container-fluid {
        height: calc(100vh - 56px);
    }

    .row {
        height: 100%;
    }

    .col-md-8, .col-md-4 {
        height: 100%;
        overflow-y: auto;
    }

    .container {
        display: flex;
        flex-direction: row;
    }
    .col-md-8 {
        flex: 2;
    }
    .col-md-4 {
        flex: 1;
    }
    .messages-panel {
        position: fixed;
        right: 0;
        top: 56px; /* Height of navbar */
        height: calc(100vh - 56px); /* Subtract navbar height */
        padding-top: 1rem; /* Remove excessive padding */
        background: white;
        border-left: 1px solid #dee2e6;
    }

    .messages-container {
        padding: 1rem;
        height: 100%;
        display: flex;
        flex-direction: column;
    }

    .messages-list {
        overflow-y: auto;
        flex: 1;
        padding-right: 0.5rem;
    }

    .main-content {
        margin-right: 33.333%; /* Adjust for fixed messages panel */
        padding-top: 1rem;
        height: calc(100vh - 56px);
        overflow-y: auto;
    }

    .alert {
        position: fixed;
        top: 1rem;
        right: 1rem;
        z-index: 1070;
        min-width: 300px;
    }
</style>