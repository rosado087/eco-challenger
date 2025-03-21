import{d as g,k}from"./chunk-XAF2PFHL.js";import{N as h,S as d,T as p,la as l}from"./chunk-YXOGGNVF.js";var I=(()=>{class s{constructor(e,t){this.document=e,this.platformId=t,this.documentIsAccessible=k(this.platformId)}static getCookieRegExp(e){let t=e.replace(/([\[\]{}()|=;+?,.*^$])/gi,"\\$1");return new RegExp("(?:^"+t+"|;\\s*"+t+")=(.*?)(?:;|$)","g")}static safeDecodeURIComponent(e){try{return decodeURIComponent(e)}catch{return e}}check(e){return this.documentIsAccessible?(e=encodeURIComponent(e),s.getCookieRegExp(e).test(this.document.cookie)):!1}get(e){if(this.check(e)){e=encodeURIComponent(e);let i=s.getCookieRegExp(e).exec(this.document.cookie);return i&&i[1]?s.safeDecodeURIComponent(i[1]):""}else return""}getAll(){if(!this.documentIsAccessible)return{};let e={},t=this.document;return t.cookie&&t.cookie!==""&&t.cookie.split(";").forEach(i=>{let[n,r]=i.split("=");e[s.safeDecodeURIComponent(n.replace(/^ /,""))]=s.safeDecodeURIComponent(r)}),e}set(e,t,i,n,r,u,m,A){if(!this.documentIsAccessible)return;if(typeof i=="number"||i instanceof Date||n||r||u||m){let f={expires:i,path:n,domain:r,secure:u,sameSite:m||"Lax",partitioned:A};this.set(e,t,f);return}let a=encodeURIComponent(e)+"="+encodeURIComponent(t)+";",o=i||{};if(o.expires)if(typeof o.expires=="number"){let f=new Date(new Date().getTime()+o.expires*1e3*60*60*24);a+="expires="+f.toUTCString()+";"}else a+="expires="+o.expires.toUTCString()+";";o.path&&(a+="path="+o.path+";"),o.domain&&(a+="domain="+o.domain+";"),o.secure===!1&&o.sameSite==="None"&&(o.secure=!0,console.warn(`[ngx-cookie-service] Cookie ${e} was forced with secure flag because sameSite=None.More details : https://github.com/stevermeister/ngx-cookie-service/issues/86#issuecomment-597720130`)),o.secure&&(a+="secure;"),o.sameSite||(o.sameSite="Lax"),a+="sameSite="+o.sameSite+";",o.partitioned&&(a+="Partitioned;"),this.document.cookie=a}delete(e,t,i,n,r="Lax"){if(!this.documentIsAccessible)return;let u=new Date("Thu, 01 Jan 1970 00:00:01 GMT");this.set(e,"",{expires:u,path:t,domain:i,secure:n,sameSite:r})}deleteAll(e,t,i,n="Lax"){if(!this.documentIsAccessible)return;let r=this.getAll();for(let u in r)r.hasOwnProperty(u)&&this.delete(u,e,t,i,n)}static{this.\u0275fac=function(t){return new(t||s)(d(g),d(l))}}static{this.\u0275prov=h({token:s,factory:s.\u0275fac,providedIn:"root"})}}return s})();var x=class s{cookieService=p(I);mapUserCookies(c){this.cookieService.set("auth_id",c.id.toString()),this.cookieService.set("auth_username",c.username),this.cookieService.set("auth_email",c.email),this.cookieService.set("auth_isAdmin",c.isAdmin.toString())}login(c,e){this.mapUserCookies(c),this.cookieService.set("auth_token",e)}logout(){this.cookieService.deleteAll()}isLoggedIn(){return!!this.cookieService.get("auth_token")}getUserInfo(){return{id:parseInt(this.cookieService.get("auth_id")),username:this.cookieService.get("auth_username"),email:this.cookieService.get("auth_email"),isAdmin:this.cookieService.get("auth_isAdmin").toLowerCase()=="true"}}getAuthToken(){return this.cookieService.get("auth_token")}updateToken(c,e){this.logout(),this.login(c,e)}static \u0275fac=function(e){return new(e||s)};static \u0275prov=h({token:s,factory:s.\u0275fac,providedIn:"root"})};export{x as a};
