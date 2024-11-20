document.addEventListener("DOMContentLoaded", e=>{
    let theoCheck = document.querySelector(".theo-span")
    fetch("http://localhost:1857/health")
    .then(resp => { if (resp && resp.ok) { resp.text() }})
    .then(data => { if (data == "ok") { theoCheck.textContent = "Let there be light"}})
    .catch(e => { theoCheck.textContent = "He who seeks shall find"})
})