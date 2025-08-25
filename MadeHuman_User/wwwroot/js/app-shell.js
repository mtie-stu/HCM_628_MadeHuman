// Requires Bootstrap 5 bundle (collapse) already loaded
(function () {
  // Ripple for sidebar links & .btn
  const rippleTargets = document.querySelectorAll('.sidebar a, .sidebar button, .btn');
  rippleTargets.forEach(el => {
    el.classList.add('ripple-parent');
    el.addEventListener('click', function (e) {
      const rect = this.getBoundingClientRect();
      const r = document.createElement('span');
      r.className = 'ripple';
      r.style.left = (e.clientX - rect.left) + 'px';
      r.style.top  = (e.clientY - rect.top)  + 'px';
      this.appendChild(r);
      setTimeout(() => r.remove(), 600);
    }, {passive:true});
  });

  // Auto open collapse that contains an active link
  document.querySelectorAll('.sidebar .collapse').forEach(col => {
    if (col.querySelector('a.active')) {
      const bsCol = bootstrap.Collapse.getOrCreateInstance(col, {toggle:false});
      bsCol.show();
      const btn = document.querySelector('[data-bs-target="#' + col.id + '"]');
      if (btn) btn.setAttribute('aria-expanded','true');
    }
  });

  // Persist collapse state
  const persistIds = ['collapseInbound','collapseOutbound','demoSubmenu'];
  persistIds.forEach(id => {
    const col = document.getElementById(id);
    if (!col) return;
    const saved = localStorage.getItem('col:'+id);
    if (saved === '1') bootstrap.Collapse.getOrCreateInstance(col,{toggle:false}).show();
    col.addEventListener('shown.bs.collapse', () => localStorage.setItem('col:'+id,'1'));
    col.addEventListener('hidden.bs.collapse', () => localStorage.setItem('col:'+id,'0'));
  });
})();
