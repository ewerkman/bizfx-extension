import { Component, OnInit, Inject, OnDestroy } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import 'rxjs/add/operator/takeWhile';

import { TranslateService } from '@ngx-translate/core';
import { ScBizFxViewsService, ScBizFxAuthService, ScBizFxContextService } from '@sitecore/bizfx';
import { Title } from '@angular/platform-browser';

/**
 * App `Component`
 */
@Component({
  // tslint:disable-next-line:component-selector
  selector: 'app-root',
  styles: [`
  i:hover {
    color: gray;
  }

  .homeicon {
    color: white
  }
`],
  templateUrl: './app.component.html'
})

export class AppComponent implements OnInit, OnDestroy {
  /**
     * @ignore
     */
  appName = '';
  /**
     * @ignore
     */
  pageTitle = 'Sitecore Experience Commerce';
  /**
     * @ignore
     */
  routes = [];
  /**
     * @ignore
     */
  isNavigationShown = true;
  /**
     * @ignore
     */
  isTaskPage = false;
  /**
     * @ignore
     */
  isHomePage = true;
  /**
     * @ignore
     */
  message: string;
  /**
       * @ignore
       */
  private alive = true;

  /**
     * @ignore
     */
  constructor(
    private viewsService: ScBizFxViewsService,
    private authService: ScBizFxAuthService,
    private router: Router,
    private bizFxContext: ScBizFxContextService,
    protected translate: TranslateService,
    private titleService: Title) { }

  /**
     * @ignore
     */
  ngOnInit() {
    this.createRoutes();

      this.viewsService.latestView$
        .takeWhile(() => this.alive)
        .subscribe(view => {
      // Check if we navigated to the current page to prevent setting invalid titles
      if (this.isNavigationShown) {
        // Check if we have a DisplayName property on the view
        if (view.Properties.length > 0) {
          const displayName = view.Properties.find(function(element) { return element.Name === 'DisplayName'; });
          // If a display name is found then use it otherwise use the default view display name
          if (displayName !== undefined && displayName.Value.length !== 0) {
            this.setTitles(displayName.Value);
          } else {
            this.setTitles(view.DisplayName);
          }
        } else {
          this.setTitles(view.DisplayName);
        }

        this.isNavigationShown = false;
      }

      this.createRoutes({
        Text: view.DisplayName
      });
    });

    this.viewsService.pageType$
      .takeWhile(() => this.alive)
      .subscribe(type => {
        Promise.resolve(null).then(() => {
          this.isTaskPage = type === 'task';
          this.isHomePage = type === 'home';
        });
      });

    this.router.events
      .takeWhile(() => this.alive)
      .subscribe((event) => {
        if (event instanceof NavigationEnd) {
          this.isNavigationShown = true;

          if (event.url === '/') {
            this.pageTitle = 'Sitecore Experience Commerce';
            this.viewsService.announcePageType('home');
            this.createRoutes();
          }
        }
      });

    this.viewsService.errorAnnounced$
      .takeWhile(() => this.alive)
      .subscribe(
        error => {
          this.message = error;
          setTimeout(() => this.message = null, 2000);
        });

    this.bizFxContext.config$
      .takeWhile(() => this.alive)
      .subscribe(config => {
        this.translate.setDefaultLang(config.Language);
        this.translate.use(this.bizFxContext.language);
      });

    this.bizFxContext.language$
      .takeWhile(() => this.alive)
      .subscribe(language => this.translate.use(language));
  }

  /**
     * @ignore
     */
  ngOnDestroy(): void {
    this.alive = false;
  }

  /**
     * @ignore
     */
  createRoutes(route?) {
    const standardRoute = {
      Link: '/',
      Text: 'COMMERCE'
    };

    const routes = [standardRoute];

    if (route) {
      routes.push(route);
    }

    this.routes = routes;
  }

  setTitles(title) {
    this.titleService.setTitle(title);
    this.pageTitle = title;
  }
}
