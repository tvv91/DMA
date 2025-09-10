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
    $("#spinner").removeClass("d-none");
    try {
        await connection.invoke("GetPosts", connection.connectionId, page, searchText, category, year, onlyDrafts);
    } catch (err) {
        console.error("Error fetching posts:", err);
        $("#spinner").addClass("d-none");
    }
}

async function loadPosts(resetPage = true, textSearch = false) {
    const searchText = $("#searchfilter").val()?.trim() || "";
    const category = $("#categoryfilter").val() || "";
    const year = $("#yearfilter").val()?.trim() || "";
    const onlyDrafts = $("#draftfilter").is(":checked");

    let page = getCurrentPage();
    if (resetPage) setCurrentPage(1);

    if ((textSearch && searchText.length >= 3) || year.length === 4 || searchText.length === 0 || !textSearch) {
        await fetchPosts(page, searchText, category, year, onlyDrafts);
    }
}

function renderPosts(posts, totalItems) {
    const $container = $("#post-container");
    const $pagination = $("#pagination");

    $container.empty();
    $pagination.empty();

    if (!posts || posts.length === 0) {
        $("#spinner").addClass("d-none");
        $container.html('<div style="text-align: center"><h4>No posts :(</h4></div>');
        return;
    }

    $("#spinner").addClass("d-none");

    posts.forEach(x => {
        $container.append(`
            <div class="${x.post.isDraft ? "post post-draft" : "post post-published"}">
                <h4><a href="post/${x.post.id}">${x.post.title}</a></h4>
                <div>${x.post.description}</div>
                <div class="blog-post-info">
                    <i>Added: <b>${x.created}</b></i>&nbsp;
                    <i>Category: <b>${x.category}</b></i>
                </div>
            </div>
        `);
    });

    for (let i = 1; i <= totalItems; i++) {
        $pagination.append(`<li class="page-item"><a id="${i}" class="page-link" href="#">${i}</a></li>`);
    }

    const currentPage = getCurrentPage();
    $(`#pagination a[id='${currentPage}']`).addClass("active");
}

function initPostListPage() {
    loadPosts();

    $("#pagination").on("click", "a.page-link", async function (e) {
        e.preventDefault();
        const selectedPage = Number(this.id);
        if (selectedPage !== getCurrentPage()) {
            setCurrentPage(selectedPage);
            await loadPosts(false);
        }
    });

    $("#searchfilter").on("input", () => loadPosts(true, true));
    $("#categoryfilter, #yearfilter, #draftfilter").on("change input", () => loadPosts());
}

async function autoSave() {
    if (!isChanged)
        return;
    isChanged = false;
    $("#spinnerbutton").removeAttr("hidden");
    $("#savebutton").prop("disabled", true);

    try {
        await connection.invoke("AutoSavePost", connection.connectionId, postId || 0,
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


connection.on("ReceivedPosts", (posts, totalItems) => renderPosts(posts, totalItems));

connection.on("PostUpdated", (updatedDate) => {
    $("#spinnerbutton").attr("hidden", true);
    $("#savebutton").prop("disabled", false);
    $("#updatedAt").text(formatDate(updatedDate));
});

connection.on("PostCreated", (newPostId, createdDate) => {
    postId = newPostId;
    $("#spinnerbutton").attr("hidden", true);
    $("#savebutton").prop("disabled", false);
    $("#updatedAt").text(formatDate(createdDate));
});

connection.onclose(async () => { await start(); });