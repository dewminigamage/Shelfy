import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _isDark = false;

  constructor() {
    const saved = localStorage.getItem('shelfy-theme');
    if (saved === 'dark') {
      this._isDark = true;
      document.body.classList.add('dark');
      document.documentElement.classList.add('dark');
    }
  }

  get isDark(): boolean {
    return this._isDark;
  }

  toggle(): void {
    this._isDark = !this._isDark;
    if (this._isDark) {
      document.body.classList.add('dark');
      document.documentElement.classList.add('dark');
      localStorage.setItem('shelfy-theme', 'dark');
    } else {
      document.body.classList.remove('dark');
      document.documentElement.classList.remove('dark');
      localStorage.setItem('shelfy-theme', 'light');
    }
  }
}
