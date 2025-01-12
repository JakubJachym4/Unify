<script lang="ts">
	import FacultyManagement from './../../lib/components/admin/UniversityCore/FacultyManagement.svelte';
	import FieldOfStudyManagement from '$lib/components/admin/shared/FieldOfStudyManagement.svelte';
    import UserManagement from '$lib/components/admin/UserManagement.svelte';
    import MessagesContainer from '$lib/components/messages/MessagesContainer.svelte';
	import LocalizationManagement from '$lib/components/admin/UniversityCore/LocalizationManagement.svelte';
	import SpecializationManagement from '$lib/components/admin/shared/SpecializationManagement.svelte';
    
    let showMessages = true;
    let activeComponent = 'users';

    const toggleMessages = () => {
        showMessages = !showMessages;
    };

    const setActiveComponent = (component: string) => {
        activeComponent = component;
    };
</script>

<div class="admin-container">
    <nav class="admin-nav bg-light border-bottom mb-4">
        <div class="container-fluid">
            <div class="d-flex justify-content-between align-items-center py-3">
                <div class="nav-buttons">
                    <button 
                        class="btn {activeComponent === 'users' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('users')}
                    >
                        User Management
                    </button>
                    <button 
                        class="btn {activeComponent === 'fieldsOfStudy' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('fieldsOfStudy')}
                    >
                        Study Fields 
                    </button>
                    <button 
                        class="btn {activeComponent === 'faculty' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('faculty')}
                    >
                        Faculties
                    </button>
                    <button 
                        class="btn {activeComponent === 'location' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('location')}
                    >
                        Locations
                    </button>
                    <button 
                        class="btn {activeComponent === 'specialization' ? 'btn-primary' : 'btn-outline-primary'} me-2"
                        on:click={() => setActiveComponent('specialization')}
                    >
                        Specializations
                    </button>
                    <!-- Add more management buttons here -->
                </div>
                <button 
                    class="btn {showMessages ? 'btn-primary' : 'btn-outline-primary'}"
                    on:click={toggleMessages}
                >
                    {showMessages ? 'Hide Messages' : 'Show Messages'}
                </button>
            </div>
        </div>
    </nav>

    <div class="content-container">
        <div class="main-content {showMessages ? 'with-messages' : ''}">
            {#if activeComponent === 'users'}
                <UserManagement />
            {:else if activeComponent === 'fieldsOfStudy'}
                <FieldOfStudyManagement />
            {:else if activeComponent === 'faculty'}
                <FacultyManagement />
            {:else if activeComponent === 'location'}
                <LocalizationManagement />
            {:else if activeComponent === 'specialization'}
                <SpecializationManagement />
            {/if}
            <!-- Add more components here -->
        </div>
        
        {#if showMessages}
            <div class="messages-sidebar">
                <MessagesContainer fixed={false} />
            </div>
        {/if}
    </div>
</div>

<style>
    .admin-container {
        height: calc(100vh - 56px);
        overflow: hidden;
    }

    .admin-nav {
        position: sticky;
        top: 0;
        z-index: 1020;
    }

    .content-container {
        display: flex;
        height: 100%;
    }

    .main-content {
        flex: 1;
        padding: 1rem;
        overflow-y: auto;
        transition: width 0.3s ease;
    }

    .main-content.with-messages {
        width: calc(100% - 400px);
    }

    .messages-sidebar {
        width: 400px;
        border-left: 1px solid #dee2e6;
        height: 100%;
        overflow-y: auto;
    }

    @media (max-width: 768px) {
        .main-content.with-messages {
            width: 100%;
        }

        .messages-sidebar {
            position: fixed;
            right: 0;
            top: 56px;
            bottom: 0;
            background: white;
            z-index: 1030;
        }
    }
</style>