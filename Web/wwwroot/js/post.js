const BACK_PAGE_POST_INDEX = "BACK_PAGE_POST_INDEX";
let isChanged = false;
let autoSaveInterval;
let postId = null;

function getCurrentPage() {
    return Number(localStorage.getItem(BACK_PAGE_POST_INDEX)) || 1;
}

function setCurrentPage(page) {
    localStorage.setItem(BACK_PAGE_POST_INDEX, page);
}

async function fetchPosts(page = 1, searchText = "", category = "", year = "", onlyDrafts = false) {
    if (postConnection.state !== signalR.HubConnectionState.Connected) {
        console.error("SignalR connection is not established");
        $("#spinner").addClass("d-none");
        return;
    }

    $("#spinner").removeClass("d-none");
    try {
        await postConnection.invoke("GetPosts", postConnection.connectionId, page, searchText, category, year, onlyDrafts);
    } catch (err) {
        console.error("Error fetching posts:", err);
        $("#spinner").addClass("d-none");
        $("#post-container").html(`
            <div style="text-align: center; padding: 60px 20px;">
                <div style="font-size: 4rem; margin-bottom: 16px;">⚠️</div>
                <h4 style="color: #dc3545; font-weight: 500; margin-bottom: 8px;">Error loading posts</h4>
                <p style="color: #6c757d; font-size: 0.9rem;">Please refresh the page to try again</p>
            </div>
        `);
    }
}

async function loadPosts(resetPage = true, textSearch = false) {
    const searchText = $("#searchfilter").val()?.trim() || "";
    const category = $("#categoryfilter").val() || "";
    const year = $("#yearfilter").val()?.trim() || "";
    const onlyDrafts = $("#draftfilter").is(":checked");

    let page = getCurrentPage();
    if (resetPage) {
        setCurrentPage(1);
        page = 1;
    }

    if ((textSearch && searchText.length >= 3) || year.length === 4 || searchText.length === 0 || !textSearch) {
        await fetchPosts(page, searchText, category, year, onlyDrafts);
    }
}

function renderPosts(posts, totalPages) {
    const $container = $("#post-container");
    const $pagination = $("#pagination");

    $container.empty();
    $pagination.empty();

    if (!posts || posts.length === 0) {
        $("#spinner").addClass("d-none");
        $container.html(`
            <div style="text-align: center; padding: 60px 20px;">
                <div style="font-size: 4rem; margin-bottom: 16px;">📝</div>
                <h4 style="color: #6c757d; font-weight: 500; margin-bottom: 8px;">No posts found</h4>
                <p style="color: #adb5bd; font-size: 0.9rem;">Try adjusting your filters or search terms</p>
            </div>
        `);
        return;
    }

    $("#spinner").addClass("d-none");

    posts.forEach(post => {
        const categories = post.categories && post.categories.length > 0 
            ? post.categories 
            : ["Uncategorized"];
        
        const postClass = post.isDraft ? "post post-draft" : "post post-published";
        const createdDate = post.created || "Unknown date";
        
        // Format date nicely
        let formattedDate = createdDate;
        try {
            const date = new Date(createdDate);
            if (!isNaN(date.getTime())) {
                formattedDate = date.toLocaleDateString('en-US', { 
                    year: 'numeric', 
                    month: 'long', 
                    day: 'numeric' 
                });
            }
        } catch (e) {
            // Keep original date if parsing fails
        }
        
        const categoryBadges = categories.map(cat => 
            `<span class="post-category-badge">${escapeHtml(cat)}</span>`
        ).join('');
        
        $container.append(`
            <div class="${postClass}">
                <h4><a href="/post/${post.id}">${escapeHtml(post.title)}</a></h4>
                <div>${escapeHtml(post.description || "")}</div>
                <div class="blog-post-info">
                    <span class="post-date-icon">
                        <span>${escapeHtml(formattedDate)}</span>
                    </span>
                    <div style="display: flex; gap: 6px; flex-wrap: wrap;">
                        ${categoryBadges}
                    </div>
                </div>
            </div>
        `);
    });

    // Render pagination
    if (totalPages > 1) {
        for (let i = 1; i <= totalPages; i++) {
            $pagination.append(`<li class="page-item"><a id="${i}" class="page-link" href="#">${i}</a></li>`);
        }

        const currentPage = getCurrentPage();
        $(`#pagination a[id='${currentPage}']`).parent().addClass("active");
    }
}

function escapeHtml(text) {
    const map = {
        '&': '&amp;',
        '<': '&lt;',
        '>': '&gt;',
        '"': '&quot;',
        "'": '&#039;'
    };
    return text ? text.replace(/[&<>"']/g, m => map[m]) : '';
}

async function fetchBlogTree() {
    if (postConnection.state !== signalR.HubConnectionState.Connected) {
        return;
    }

    try {
        await postConnection.invoke("GetBlogTree", postConnection.connectionId);
    } catch (err) {
        console.error("Error fetching blog tree:", err);
    }
}

function renderBlogTree(tree) {
    const $tree = $("#blog-tree");
    $tree.empty();

    if (!tree || tree.length === 0) {
        $tree.html('<div class="text-muted">No posts available</div>');
        return;
    }

    tree.forEach(category => {
        const $category = $('<div class="tree-category"></div>');
        const $categoryHeader = $(`
            <div class="tree-category-header">
                <i class="fa fa-folder tree-icon"></i>
                <span class="tree-category-name">${escapeHtml(category.category)}</span>
            </div>
        `);
        
        $categoryHeader.on("click", function() {
            $(this).parent().find(".tree-year-group").slideToggle();
            $(this).find(".tree-icon").toggleClass("fa-folder fa-folder-open");
        });

        $category.append($categoryHeader);

        if (category.posts && category.posts.length > 0) {
            const $yearGroups = $('<div class="tree-year-group"></div>');
            
            category.posts.forEach(yearGroup => {
                const $year = $('<div class="tree-year"></div>');
                const $yearHeader = $(`
                    <div class="tree-year-header">
                        <i class="fa fa-calendar tree-icon"></i>
                        <span class="tree-year-name">${yearGroup.year}</span>
                    </div>
                `);
                
                $yearHeader.on("click", function() {
                    $(this).parent().find(".tree-posts").slideToggle();
                    $(this).find(".tree-icon").toggleClass("fa-calendar fa-calendar-check");
                });

                $year.append($yearHeader);

                if (yearGroup.posts && yearGroup.posts.length > 0) {
                    const $posts = $('<div class="tree-posts"></div>');
                    
                    yearGroup.posts.forEach(post => {
                        const $post = $(`
                            <div class="tree-post">
                                <a href="/post/${post.id}" class="tree-post-link">${escapeHtml(post.title)}</a>
                                <span class="tree-post-date">${post.created}</span>
                            </div>
                        `);
                        $posts.append($post);
                    });

                    $year.append($posts);
                }

                $yearGroups.append($year);
            });

            $category.append($yearGroups);
        }

        $tree.append($category);
    });
}

function initPostListPage() {
    // Reset to page 1 if coming from post creation/update
    if (sessionStorage.getItem("post_created") === "1" || sessionStorage.getItem("post_updated") === "1") {
        setCurrentPage(1);
        sessionStorage.removeItem("post_created");
        sessionStorage.removeItem("post_updated");
    }
    
    // Load posts - connection should already be started by document.ready
    if (postConnection.state === signalR.HubConnectionState.Connected) {
        loadPosts();
        fetchBlogTree();
    } else {
        // Wait a bit for connection to establish, then load posts
        setTimeout(() => {
            if (postConnection.state === signalR.HubConnectionState.Connected) {
                loadPosts();
                fetchBlogTree();
            } else {
                console.warn("SignalR connection not ready, retrying...");
                postConnection.start().then(() => {
                    loadPosts();
                    fetchBlogTree();
                }).catch(err => {
                    console.error("Error starting SignalR connection:", err);
                    $("#spinner").addClass("d-none");
                    $("#post-container").html(`
                        <div style="text-align: center; padding: 60px 20px;">
                            <div style="font-size: 4rem; margin-bottom: 16px;">🔌</div>
                            <h4 style="color: #dc3545; font-weight: 500; margin-bottom: 8px;">Connection Error</h4>
                            <p style="color: #6c757d; font-size: 0.9rem;">Unable to connect to server. Please refresh the page.</p>
                        </div>
                    `);
                });
            }
        }, 100);
    }

    $("#pagination").on("click", "a.page-link", async function (e) {
        e.preventDefault();
        const selectedPage = Number($(this).attr("id"));
        const currentPage = getCurrentPage();
        if (selectedPage !== currentPage) {
            setCurrentPage(selectedPage);
            await loadPosts(false);
        }
    });

    $("#searchfilter").on("input", debounce(() => loadPosts(true, true), 500));
    $("#categoryfilter, #yearfilter, #draftfilter").on("change", () => loadPosts(true));
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

async function autoSave() {
    if (!isChanged)
        return;
    isChanged = false;
    $("#spinnerbutton").removeAttr("hidden");
    $("#savebutton").prop("disabled", true);

    try {
        await postConnection.invoke("AutoSavePost", postConnection.connectionId, postId || 0,
            $("#title").val().trim(),
            $("#description").val().trim(),
            $("#content").val().trim(),
            $("#category").val()
        );
    } catch (err) {
        console.error("Error sending AutoSavePost:", err);
    }
}

function onChange() {
    const title = $("#title").val()?.trim();
    const description = $("#description").val()?.trim();
    const content = $("#content").val()?.trim();
    const category = $("#category").val();

    if (title && description && content && category && category !== "Category") {
        $("#savebutton").prop("disabled", false);
        isChanged = true;
    } else {
        $("#savebutton").prop("disabled", true);
        isChanged = false;
    }
}

function setupPreview() {
    $("#previewButton").on("click", () => {
        const safeHtml = DOMPurify.sanitize($("#content").val());
        $(".modal-body").html(safeHtml);
    });
}

function initPostEditorPage() {
    postId = Number($("#Id").val()) || 0;
    autoSaveInterval = setInterval(autoSave, 30000);
    $(window).on("beforeunload", () => clearInterval(autoSaveInterval));

    setupPreview();
    $("#title, #description, #content, #category").on("input change", onChange);
}


async function onRemove(postId) {
    try {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        const token = tokenElement ? tokenElement.value : "";

        const response = await fetch(`/post/delete?id=${postId}`, {
            method: "DELETE",
            headers: {
                "RequestVerificationToken": token
            }
        });

        if (response.ok) {
            window.location.href = "/";
        } else {
            alert("Failed to delete post");
        }
    } catch (err) {
        console.error("Error removing post:", err);
    }
}

function initViewPostPage() {
    $(".remove-post-btn").on("click", function () {
        const id = $(this).data("id");
        if (confirm("This action cannot be undone. Are you sure you want to remove the post?")) {
            onRemove(id);
        }
    });
}
function formatDate(dateString) {
    const date = new Date(dateString);
    return `Automatically saved: ${date.toLocaleDateString()}, ${date.toLocaleTimeString()}`;
}

$("#savebutton").on("click", function () {
    autoSave();
});

$(document).ready(async () => {
    try {
        await start();
    } catch (err) {
        console.error("Error starting SignalR connection:", err);
    }

    const page = $("body").data("page");

    if (page === "post-list") initPostListPage();
    else if (page === "view-post") initViewPostPage();
    else if (page === "new-post") initPostEditorPage();
});


postConnection.on("ReceivedPosts", (posts, totalPages) => {
    renderPosts(posts, totalPages);
});

postConnection.on("ReceivedBlogTree", (tree) => {
    renderBlogTree(tree);
});

postConnection.on("PostUpdated", (updatedDate) => {
    $("#spinnerbutton").attr("hidden", true);
    $("#savebutton").prop("disabled", false);
    $("#updatedAt").text(formatDate(updatedDate));
});

postConnection.on("PostCreated", (newPostId, createdDate) => {
    postId = newPostId;
    $("#spinnerbutton").attr("hidden", true);
    $("#savebutton").prop("disabled", false);
    $("#updatedAt").text(formatDate(createdDate));
});

postConnection.onclose(async () => { await start(); });