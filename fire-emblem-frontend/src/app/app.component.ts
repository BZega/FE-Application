import { CommonModule, isPlatformBrowser } from '@angular/common';
import { Component, Inject, PLATFORM_ID } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { PageNavigationComponent } from './core/navigation/page-navigation.component';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet, 
    CommonModule,
    PageNavigationComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  constructor(@Inject(PLATFORM_ID) private platformId: object) {}
  title = 'fe-frontend';

  ngOnInit() {
    this.detectTheme();
  }

  changeTheme(event: Event) {
    const selectedTheme = (event.target as HTMLSelectElement).value;
    this.applyTheme(selectedTheme);
  }
  
  applyTheme(theme: string) {
    if (isPlatformBrowser(this.platformId)) {
      document.documentElement.setAttribute('data-theme', theme);
    }
  }

  detectTheme() {
      const prefersDark = true;
      this.applyTheme(prefersDark ? 'dark' : 'light');
  }
}
