import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { ThemeService } from '../services/theme.service';

@Component({
  selector: 'app-page-navigation',
  imports: [CommonModule, RouterModule],
  templateUrl: './page-navigation.component.html',
  styleUrl: './page-navigation.component.scss'
})
export class PageNavigationComponent {
  isMenuOpen = false;
  isDarkMode$: Observable<boolean>;
  isCharacterDetailPage$: Observable<boolean>;

  constructor(
    private themeService: ThemeService,
    private router: Router
  ) {
    this.isDarkMode$ = this.themeService.darkMode$;
    
    // Check if we're on character detail page
    this.isCharacterDetailPage$ = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => this.router.url.includes('/character-detail/'))
    );
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }

  toggleTheme(): void {
    this.themeService.toggleTheme();
  }

  navigateToCharacterTab(tab: string): void {
    // Emit event or use a service to communicate with character detail component
    window.dispatchEvent(new CustomEvent('characterTabChange', { detail: tab }));
    this.closeMenu();
  }
}