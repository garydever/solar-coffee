import Vue from "vue";
import Vuex from "vuex";
import pathify from 'vuex-pathify';

pathify.options.mapping = 'simple';
pathify.options.deep = 2;


Vue.use(Vuex);

export default new Vuex.Store({
  state: {},
  mutations: {},
  actions: {},
  modules: {},
  plugins: [pathify.plugin]
});
